using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsonPatchForDotnet
{
    public static class PathResolver
    {
        public static IEnumerable<object> GetProperties(this object o, string[] paths)
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
        
        public static IEnumerable<object> GetProperties(this object o, string path)
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
            var root = path.Split('[');
            return (root.ElementAt(0), root.ElementAtOrDefault(1));
        }
    }
}