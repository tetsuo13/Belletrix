using System;
using System.Configuration;

namespace Belletrix.Core
{
    /// <summary>
    /// Environment awareness.
    /// </summary>
    public static class DeploymentEnvironment
    {
        /// <summary>
        /// "Environment" app setting value.
        /// </summary>
        public static string Enviroment
        {
            get
            {
                string environment = null;

                try
                {
                    environment = ConfigurationManager.AppSettings["Environment"];
                }
                catch (Exception)
                {
                }

                return environment;
            }
        }

        public static bool IsDevelopment { get { return Enviroment == "Debug"; } }
        public static bool IsProduction { get { return Enviroment == "Release"; } }
    }
}
