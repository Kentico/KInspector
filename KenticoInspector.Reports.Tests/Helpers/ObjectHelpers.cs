using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KenticoInspector.Reports.Tests.Helpers
{
    static class ObjectHelpers
    {
        public static bool ObjectHasPropertyWithExpectedValue<T>(object objectToCheck, string propertyName, IEnumerable<T> expectedValue)
        {
            var objectPropertyValue = objectToCheck.GetPropertyValue<IEnumerable<T>>(propertyName);
            return objectPropertyValue.SequenceEqual(expectedValue);
        }
    }
}
