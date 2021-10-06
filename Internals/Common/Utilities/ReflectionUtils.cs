using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BaselessJumping.Internals.Common.Utilities
{
    public static class ReflectionUtils
    {
        #nullable enable
        public static T? GetValueAs<T>(this FieldInfo info, object? obj) where T : class
        {
            return info?.GetValue(obj) as T;
        }
        #nullable disable
    }
}
