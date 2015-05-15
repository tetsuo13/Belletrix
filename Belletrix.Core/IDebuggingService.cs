
namespace Belletrix.Core
{
    /// <summary>
    /// Interface for defining services to determine if the code is running in
    /// DEBUG configuration. Similar to "#if DEBUG" preprocessor directive.
    /// </summary>
    public interface IDebuggingService
    {
        /// <summary>
        /// Running in DEBUG?
        /// </summary>
        /// <returns><see langword="true"/> if DEBUG.</returns>
        bool RunningInDebugMode();
    }
}
