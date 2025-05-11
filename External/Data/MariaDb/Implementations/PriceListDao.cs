using GalliumPlus.Core.Data;
using GalliumPlus.Core.Exceptions;
using GalliumPlus.Core.Stocks;
using KiwiQuery;
using KiwiQuery.Mapped;
using MySqlConnector;

namespace GalliumPlus.Data.MariaDb.Implementations;

public class PriceListDao : Dao, IPriceListDao
{
    public PriceListDao(DatabaseConnector connector) : base(connector)
    {
    }

    public PriceList Create(PriceList client)
    {
        throw new NotImplementedException();
    }

    public void Delete(int key)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PriceList> Read()
    {
        using MySqlConnection connection = this.Connect();
        Schema db = new(connection);

        return db.Select<PriceList>().Where(db.Column("applicableDuring") == 1).FetchList();
    }

    public PriceList Read(int key)
    {
        using MySqlConnection connection = this.Connect();
        Schema db = new(connection);

        var found = db.Select<PriceList>().WhereAll(db.Column("id") == key, db.Column("applicableDuring") == 1).FetchList();

        if (found.Count == 1)
        {
            return found[0];
        }
        else
        {
            throw new ItemNotFoundException();
        }
    }

    public PriceList Update(int key, PriceList item)
    {
        using var connection = this.Connect();
        Schema db = new(connection);

        bool ok = db.Update<PriceList>().SetInserted(item).Where(db.Column("id") == key).Apply();

        if (!ok) throw new ItemNotFoundException("Cette catégorie");

        return new PriceList(key, item.ShortName, item.LongName, item.RequiresMembership);
    }
}