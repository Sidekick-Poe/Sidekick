using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sidekick.Common.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Get the specified attribute from the specified type
        /// </summary>
        /// <typeparam name="TAttribute">The attribute to get from the type</typeparam>
        /// <param name="type">The type from which to find the attribute</param>
        /// <returns>The attribute, or null</returns>
        public static TAttribute GetAttribute<TAttribute>(this Type type)
        {
            var attributeType = typeof(TAttribute);
            return (TAttribute)type.GetCustomAttributes(false).FirstOrDefault(x => attributeType.IsInstanceOfType(x));
        }

        /// <summary>
        /// Get all instances of the specified attributes from the specified type
        /// </summary>
        /// <typeparam name="TAttribute">The attribute to get from the type</typeparam>
        /// <param name="type">The type from which to find the attributes</param>
        /// <returns>The list of attributes</returns>
        public static IList<TAttribute> GetAttributes<TAttribute>(this Type type)
        {
            var attributeType = typeof(TAttribute);
            return type.GetCustomAttributes(false)
              .Where(x => attributeType.IsInstanceOfType(x))
              .Select(x => (TAttribute)x)
              .ToList();
        }

        /// <summary>
        /// Get all types implementing the interface
        /// </summary>
        /// <param name="interface">The interface to find on types</param>
        /// <returns>The list of types implementing the interface</returns>
        public static List<Type> GetTypesImplementingInterface(this Type @interface)
        {
            return FindTypes(x => x.GetInterfaces().Contains(@interface));
        }

        /// <summary>
        /// Get all types implementing the attribute
        /// </summary>
        /// <param name="attribute">The attribute to find on types</param>
        /// <returns>The list of types implementing the attribute</returns>
        public static List<Type> GetTypesImplementingAttribute(this Type attribute)
        {
            return FindTypes(x => x.GetCustomAttributes(false).Any(y => attribute.IsInstanceOfType(y)));
        }

        private static List<Type> FindTypes(Func<Type, bool> func)
        {
            var results = new List<Type>();
            var executedAssemblies = new List<string>();
            FindTypes(ref results, ref executedAssemblies, func, AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName()).ToArray());
            return results;
        }

        private static void FindTypes(ref List<Type> results, ref List<string> executedAssemblies, Func<Type, bool> func, AssemblyName[] assemblies)
        {
            foreach (var assemblyName in assemblies.Where(x => x.FullName.StartsWith("Sidekick")))
            {
                if (executedAssemblies.Contains(assemblyName.FullName))
                {
                    continue;
                }

                executedAssemblies.Add(assemblyName.FullName);

                try
                {
                    var assembly = Assembly.Load(assemblyName);

                    foreach (var type in assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface).Where(func))
                    {
                        if (!results.Any(x => x.FullName == type.FullName))
                        {
                            results.Add(type);
                        }
                    }

                    FindTypes(ref results, ref executedAssemblies, func, assembly.GetReferencedAssemblies());
                }
                catch (Exception)
                {
                    // If an assembly can't be loaded, we skip it. It hasn't caused issues yet.
                }
            }
        }
    }
}
