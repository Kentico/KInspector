using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenticoInspector.Reports.Tests.Helpers
{
    static class ObjectHelpers
    {
        public static bool ObjectPropertyValueEqualsExpectedValue<T>(object objectWithProperty, string propertyName, IEnumerable<T> expectedValue)
        {
            var objectPropertyValue = objectWithProperty.GetPropertyValue<IEnumerable<T>>(propertyName);
            return objectPropertyValue.SequenceEqual(expectedValue);
        }
    }
}
