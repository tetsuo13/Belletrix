using System.Configuration;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Bennett.AbroadAdvisor.Core
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

            public static bool connection_ValidateRemoteCertificateCallback(X509Certificate cert, X509Chain chain,
                SslPolicyErrors errors)
            {
                return cert.Subject.Contains("bluehost.com");
            }
        }
    }
}
