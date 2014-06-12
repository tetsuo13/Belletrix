using System;
using System.Configuration;

namespace Bennett.AbroadAdvisor.Core
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

        /// <summary>
        /// If the environment seems to be the development one.
        /// </summary>
        public static bool IsDevelopment
        {
            get
            {
                return Enviroment == "Debug";
            }
        }
    }
}
