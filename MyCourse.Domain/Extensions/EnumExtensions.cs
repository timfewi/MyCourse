using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum? value)
        {
            if (value is null)
                return string.Empty;

            FieldInfo? field = value.GetType().GetField(value.ToString());

            if (field != null)
            {
                DisplayAttribute? attribute = field.GetCustomAttribute<DisplayAttribute>();

                if (!string.IsNullOrEmpty(attribute?.Name))
                {
                    return attribute.Name;
                }
            }

            return value.ToString();
        }
    }
}
