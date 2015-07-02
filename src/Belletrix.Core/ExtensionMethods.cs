using System.Data.Common;
using System.Threading.Tasks;

namespace Belletrix.Core
{
    public static class ExtensionMethods
    {
        public static async Task<string> GetText(this DbDataReader reader, string columnName)
        {
            int ord = reader.GetOrdinal(columnName);

            if (await reader.IsDBNullAsync(ord))
            {
                return null;
            }
            return await reader.GetFieldValueAsync<string>(ord);
        }
    }
}
