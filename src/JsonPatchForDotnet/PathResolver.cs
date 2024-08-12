using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime;
using JsonPatchForDotnet.Extensions;
using JsonPatchForDotnet.Queries;
using ScimPatch.Antlr;

namespace JsonPatchForDotnet
{
    public static class PathResolver
    {
        /// <summary>
        /// This will process the path and filters to find all relevant properties.
        /// </summary>
        /// <param name="o">The instance.</param>
        /// <param name="paths">Each part of the object path as an array.</param>
        /// <returns>All objects of o that satisfy the path and filters specified.</returns>
        public static IList<object> GetProperties(this object o, string[] paths)
        {
            var objects = new List<object>();

            var path = paths[0];
            var children = GetProperties(o, path);
            
            if (paths.Length > 1)
            {
                var childPaths = paths.Skip(1).ToArray();
                foreach (var child in children)
                {
                    if (Utils.IsIEnumerable(child.GetType()))
                    {
                        foreach (var item in (IEnumerable)child)
                        {
                            objects.AddRange(GetProperties(item, childPaths));
                        }
                    }
                    else
                    {
                        objects.AddRange(GetProperties(child, childPaths));
                    }
                }
            }
            else
            {
                objects.AddRange(children);
            }

            return objects;
        }
        
        internal static IEnumerable<object> GetProperties(this object o, string path)
        {
            var (root, filter) = GetRootPath(path);

            var type = o.GetType();
            
            if (type.IsNonStringEnumerable())
            { 
                foreach (var item in (IEnumerable<object>)o)
                {
                    yield return ApplyFilterIfPossible(item.GetType().GetProperty(root)!.GetValue(item), filter);
                }
            }
            else
            {
                yield return ApplyFilterIfPossible(o.GetType().GetProperty(root)!.GetValue(o), filter);
            }
        }

        private static object ApplyFilterIfPossible(object o, string? filter)
        {
            var type = o.GetType();

            if (type.IsNonStringEnumerable() && !string.IsNullOrEmpty(filter))
            {
                var enumerableType = type.GetGenericArguments()[0];
                var parser = CreateParser(filter);
                var scimFilterVisitorGenericType = typeof(ScimFilterVisitor<>).MakeGenericType(enumerableType);
                object visitor = Activator.CreateInstance(scimFilterVisitorGenericType);
                var filterContext = parser.filter();
                var expression = (LambdaExpression)scimFilterVisitorGenericType
                    .GetMethod("Visit")!
                    .Invoke(visitor, new object[] { filterContext });
                var compiledResult = expression.Compile();
                var enumerable = (IEnumerable<object>)o;
                return enumerable.Where(e => (bool)compiledResult.DynamicInvoke(e));
            }

            return o;
        }
        
        private static ScimFilterParser CreateParser(string filter)
        {
            var inputStream = new AntlrInputStream(filter);
            var speakLexer = new ScimFilterLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(speakLexer);
            var speakParser = new ScimFilterParser(commonTokenStream);
            return speakParser;   
        }

        private static (string, string?) GetRootPath(string path)
        {
            var bracketStart = path.IndexOf('[');
            var bracketEnd = path.IndexOf(']');
            string? filter = null;
            var root = path;
            if (bracketStart > 1 && bracketEnd + 1 > bracketStart - 1)
            {
                root = path.Substring(0, bracketStart);
                filter = path.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
            }
            return (root, filter);
        }
    }
}