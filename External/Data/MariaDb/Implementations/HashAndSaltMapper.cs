using System.Data;
using GalliumPlus.Core.Security;
using KiwiQuery.Mapped.Extension;
using KiwiQuery.Mapped.Mappers.Fields;

namespace GalliumPlus.Data.MariaDb;

[SharedMapper]
public class HashAndSaltMapper : IFieldMapper
{
    public bool CanHandle(Type fieldType) => fieldType == typeof(PasswordInformation) || fieldType == typeof(OneTimeSecret);

    public IFieldMapper SpecializeFor(Type fieldType, IColumnInfo info, IFieldMapperCollection collection)
    {
        if (fieldType == typeof(PasswordInformation))
        {
            return new SpecializedForPasswordInformation();
        }
        else if (fieldType == typeof(OneTimeSecret))
        {
            return new SpecializedForOneTimeSecret();
        }
        else
        {
            throw new InvalidOperationException($"Les champs de type {fieldType.FullName} ne sont pas pris en charge.");
        }
    }

    public object? ReadValue(IDataRecord record, int offset) =>
        throw new InvalidOperationException("Ce mapper ne peut pas être utilisé sans spécialisation.");

    public IEnumerable<object?> WriteValue(object? fieldValue) =>
        throw new InvalidOperationException("Ce mapper ne peut pas être utilisé sans spécialisation.");

    public IEnumerable<string> MetaColumns => ["salt"];

    public bool CanMapIntegerKey => false;

    private abstract class Specialized<T> : IFieldMapper
    {
        public bool CanHandle(Type fieldType) => fieldType == typeof(T);

        public IFieldMapper SpecializeFor(Type fieldType, IColumnInfo info, IFieldMapperCollection collection) => this;

        public object? ReadValue(IDataRecord record, int offset)
        {
            byte[] hash = new byte[32];
            record.GetBytes(offset, 0, hash, 0, 32);
            string salt = record.GetString(offset + 1);
            return this.FromHashAndSalt(hash, salt);
        }

        public IEnumerable<object?> WriteValue(object? fieldValue) =>
            [this.GetHash((T)fieldValue!), this.GetSalt((T)fieldValue!)];

        public IEnumerable<string> MetaColumns => ["salt"];

        public bool CanMapIntegerKey => false;

        protected abstract T FromHashAndSalt(byte[] hash, string salt);

        protected abstract byte[] GetHash(T value);

        protected abstract string GetSalt(T value);
    }

    private class SpecializedForOneTimeSecret : Specialized<OneTimeSecret>
    {
        protected override OneTimeSecret FromHashAndSalt(byte[] hash, string salt) => new(hash, salt);

        protected override byte[] GetHash(OneTimeSecret value) => value.Hash;

        protected override string GetSalt(OneTimeSecret value) => value.Salt;
    }

    private class SpecializedForPasswordInformation : Specialized<PasswordInformation>
    {
        protected override PasswordInformation FromHashAndSalt(byte[] hash, string salt) => new(hash, salt);

        protected override byte[] GetHash(PasswordInformation value) => value.Hash;

        protected override string GetSalt(PasswordInformation value) => value.Salt;
    }
}