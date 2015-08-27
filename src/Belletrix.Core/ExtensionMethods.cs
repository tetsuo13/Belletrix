using System.Data.Common;
using System.Threading.Tasks;

namespace Belletrix.Core
{
    public static class ExtensionMethods
    {
        public static async Task<T> GetValueOrDefault<T>(this DbDataReader reader, string columnName)
        {
            int ord = reader.GetOrdinal(columnName);

            if (await reader.IsDBNullAsync(ord))
            {
                return default(T);
            }
            return await reader.GetFieldValueAsync<T>(ord);
        }
    }
}
