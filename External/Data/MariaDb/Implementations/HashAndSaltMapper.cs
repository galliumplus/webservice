using System.Data;
using GalliumPlus.Core.Security;
using KiwiQuery.Mapped.Extension;
using KiwiQuery.Mapped.Mappers.Fields;

namespace GalliumPlus.Data.MariaDb.Implementations;

public abstract class HashAndSaltMapper<T> : IFieldMapper
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

[SharedMapper]
public class OneTimeSecretMapper : HashAndSaltMapper<OneTimeSecret>
{
    protected override OneTimeSecret FromHashAndSalt(byte[] hash, string salt) => new(hash, salt);

    protected override byte[] GetHash(OneTimeSecret value) => value.Hash;

    protected override string GetSalt(OneTimeSecret value) => value.Salt;
}

[SharedMapper]
public class PasswordInformationMapper : HashAndSaltMapper<PasswordInformation>
{
    protected override PasswordInformation FromHashAndSalt(byte[] hash, string salt) => new(hash, salt);

    protected override byte[] GetHash(PasswordInformation value) => value.Hash;

    protected override string GetSalt(PasswordInformation value) => value.Salt;
}