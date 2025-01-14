namespace KInspector.Tests.Common.Helpers
{
    public static class ObjectHelpers
    {
        public static bool ObjectHasPropertyWithExpectedValue<T>(object objectToCheck, string propertyName, IEnumerable<T> expectedValue)
        {
            var objectPropertyValue = objectToCheck.GetPropertyValue<IEnumerable<T>>(propertyName);
            if (objectPropertyValue is null)
            {
                return false;
            }

            return objectPropertyValue.SequenceEqual(expectedValue);
        }
    }
}