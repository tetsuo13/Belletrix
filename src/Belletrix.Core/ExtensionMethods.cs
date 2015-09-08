using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Gets the
        /// <see cref="System.ComponentModel.DataAnnotations.DisplayAttribute"/>
        /// value's Name value from an Enum. If the particular value doesn't
        /// have one then the value is converted to a
        /// <see cref="System.String"/>.
        /// </summary>
        /// <param name="enumValue">Enum value to convert.</param>
        /// <returns>Enum's Name value if applicable.</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            DisplayAttribute attribute = enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            if (attribute == null)
            {
                return enumValue.ToString();
            }

            return attribute.GetName();
        }

        public static string CapitalizeFirstLetter(this string value)
        {
            value = value.Trim();

            if (value.Length == 1)
            {
                return value.ToUpper();
            }

            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }
    }
}
