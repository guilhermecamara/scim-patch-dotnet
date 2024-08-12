using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            
            if (Utils.IsIEnumerable(type))
            {
                // TODO apply filter

                foreach (var item in (IEnumerable<object>)o)
                {
                    yield return item.GetType().GetProperty(root)!.GetValue(item);
                }
            }
            else
            {
                yield return o.GetType().GetProperty(root)!.GetValue(o);
            }
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