using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Modules.Tests.Helpers
{
    public static class ObjectHelpers
    {
        public static bool ObjectHasPropertyWithExpectedValue<T>(object objectToCheck, string propertyName, IEnumerable<T> expectedValue)
        {
            var objectPropertyValue = objectToCheck.GetPropertyValue<IEnumerable<T>>(propertyName);

            return objectPropertyValue.SequenceEqual(expectedValue);
        }
    }
}