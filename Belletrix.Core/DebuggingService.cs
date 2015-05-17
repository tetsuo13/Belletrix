using System.Diagnostics;

namespace Belletrix.Core
{
    /// <summary>
    /// Debugging service to detect when running application in DEBUG
    /// configuration.
    /// </summary>
    /// <remarks>
    /// Can be stubbed out in unit tests to test both code paths.
    /// </remarks>
    public class DebuggingService : IDebuggingService
    {
        /// <summary>
        /// By default, we're not running in debug mode.
        /// </summary>
        private bool debugging;

        /// <summary>
        /// Running in DEBUG?
        /// </summary>
        /// <returns><see langword="true"/> if DEBUG.</returns>
        public bool RunningInDebugMode()
        {
            CheckIfDebugging();
            return debugging;
        }

        /// <summary>
        /// Method only executes if the DEBUG variable is set. If the method
        /// executes then it'll set the debugging flag to
        /// <see langword="true"/>.
        /// </summary>
        [Conditional("DEBUG")]
        private void CheckIfDebugging()
        {
            debugging = true;
        }
    }
}
