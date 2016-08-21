using Belletrix.Entity.Enum;
using System;
using System.Collections.Generic;

namespace Belletrix.Core
{
    /// <summary>
    /// Various formatting routines.
    /// </summary>
    public static class Formatter
    {
        /// <summary>
        /// Format a phone number into parts.
        /// </summary>
        /// <remarks>
        /// Phone number is expected to be 10 digits -- area code, prefix, and
        /// line number.
        /// </remarks>
        /// <param name="number">10-digit phone number.</param>
        /// <returns>Formatted number.</returns>
        public static string PhoneNumber(string number)
        {
            if (String.IsNullOrWhiteSpace(number) || number.Length != 10)
            {
                return number;
            }
            return String.Format("{0:(###) ###-####}", Double.Parse(number));
        }

        /// <summary>
        /// Get a label CSS class name.
        /// </summary>
        /// <param name="type">Activity log type.</param>
        /// <param name="typeLabels">
        /// Dictionary returned by
        /// <see cref="Belletrix.Domain.ActivityService.GetActivityTypeLabels"/>.
        /// </param>
        /// <returns>Suitable label name.</returns>
        public static string ActivityLogLabel(ActivityLogTypes type, IDictionary<int, string> typeLabels)
        {
            int i = (int)type % typeLabels.Count;
            return typeLabels.ContainsKey(i) ? typeLabels[i] : typeLabels[1];
        }
    }
}
