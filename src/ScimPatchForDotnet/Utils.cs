using System;
using System.Collections;
using System.Collections.Generic;

namespace ScimPatchForDotnet
{
    public static class Utils
    {
        public static bool IsIEnumerable(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                // If it's a nullable type, use its underlying type for further checks
                type = underlyingType;
            }
            
            return type.IsGenericType &&
                   (typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                   || typeof(IEnumerable).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }
        
        public static bool IsIList(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                // If it's a nullable type, use its underlying type for further checks
                type = underlyingType;
            }
            
            return type.IsGenericType &&
                   (typeof(IList<>).IsAssignableFrom(type.GetGenericTypeDefinition())
                   || typeof(IList).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }
    }
}