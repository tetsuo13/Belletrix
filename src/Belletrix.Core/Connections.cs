using System.Configuration;

namespace Belletrix.Core
{
    /// <summary>
    /// Various connection settings.
    /// </summary>
    public static class Connections
    {
        /// <summary>
        /// Database connection settings.
        /// </summary>
        public static class Database
        {
            /// <summary>
            /// standard DSN for database connection.
            /// </summary>
            public static string Dsn
            {
                get
                {
                    return ConfigurationManager.ConnectionStrings["Production"].ConnectionString;
                }
            }
        }
    }
}
