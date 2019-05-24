using System;
using System.Collections.Generic;
using System.Text;

namespace KenticoInspector.Reports.Tests.Helpers
{
    static class ReflectionExtensions
    {
        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}
