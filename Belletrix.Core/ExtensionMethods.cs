using Npgsql;

namespace Belletrix.Core
{
    public static class ExtensionMethods
    {
        public static string GetText(this NpgsqlDataReader reader, string columnName)
        {
            int ord = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ord))
            {
                return null;
            }
            return reader.GetString(ord);
        }
    }
}
