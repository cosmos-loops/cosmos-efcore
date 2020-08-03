using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

/*
 * Reference to:
 *     https://github.com/PaulARoy/EntityFrameworkCore.RawSQLExtensions
 * Author:
 *     Paul Roy
 */

namespace Cosmos.EntityFrameworkCore.Core.RawQuery
{
    public interface ISqlQuery<T>
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