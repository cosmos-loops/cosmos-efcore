using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Cosmos.EntityFrameworkCore.SqlRaw
{
    public interface ISqlRawQuery<T>
    {
        Task<IList<T>> ToListAsync();

        Task<T> FirstAsync();

        Task<T> FirstOrDefaultAsync();

        Task<T> SingleAsync();

        Task<T> SingleOrDefaultAsync();

        IList<T> ToList();

        T First();

        T FirstOrDefault();

        T Single();

        T SingleOrDefault();

        DataTable ToDataTable();

        Task<DataTable> ToDataTableAsync();
    }
}