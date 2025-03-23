using StaffApp.Application.DTOs.Common;
using System.ComponentModel;
using System.Reflection;

namespace StaffApp.Application.Extensions.Helpers
{
    public class EnumHelper
    {
        /// <summary>
        /// Gets the description attribute value for an enum value.
        /// If no description attribute is found, returns the enum value as a string.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description or the enum value as a string.</returns>
        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attribute = fi.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// Generates a list of DropDownDTO objects from an enum type.
        /// Each DropDownDTO contains the enum value's integer ID and its description.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <returns>A list of DropDownDTO objects.</returns>
        public static List<DropDownDTO> GetDropDownList<TEnum>() where TEnum : Enum
        {
            return ((TEnum[])Enum.GetValues(typeof(TEnum)))
                    .Select(enumValue => new DropDownDTO
                    {
                        Id = Convert.ToInt32(enumValue),
                        Name = GetEnumDescriptionByEnum(enumValue)
                    })
                    .ToList();
        }

        /// <summary>
        /// Helper method to retrieve the enum description by enum value.
        /// If no description attribute is found, returns the enum value as a string.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description or the enum value as a string.</returns>
        private static string GetEnumDescriptionByEnum(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attribute = fi.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}
