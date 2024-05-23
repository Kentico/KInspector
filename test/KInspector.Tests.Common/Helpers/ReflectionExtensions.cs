namespace KInspector.Tests.Common.Helpers
{
    public static class ReflectionExtensions
    {
        public static T? GetPropertyValue<T>(this object obj, string propertyName)
        {
            var value = obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
            if (value is null)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}