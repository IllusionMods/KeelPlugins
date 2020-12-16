namespace System.Reflection
{
    public static class ReflectionExtensions
    {
        public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit) as T[];
        }
    }
}
