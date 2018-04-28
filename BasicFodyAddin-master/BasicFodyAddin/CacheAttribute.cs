using System;

namespace BasicFodyAddin
{
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CacheAttribute : Attribute
    {
    }
}
