using System.Reflection;

namespace Sidekick.Common.Extensions;

/// <summary>
///     Class containing extension methods for types.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Get all types implementing the interface
    /// </summary>
    /// <param name="interface">The interface to find on types</param>
    /// <returns>The list of types implementing the interface</returns>
    public static List<Type> GetTypesImplementingInterface(this Type @interface)
    {
        return FindTypes(
            x => x
                 .GetInterfaces()
                 .Contains(@interface));
    }

    private static List<Type> FindTypes(Func<Type, bool> func)
    {
        var results = new List<Type>();
        var executedAssemblies = new List<string>();
        FindTypes(
            ref results,
            ref executedAssemblies,
            func,
            assemblies: AppDomain
                        .CurrentDomain.GetAssemblies()
                        .Select(x => x.GetName())
                        .ToArray());
        return results;
    }

    private static void FindTypes(
        ref List<Type> results,
        ref List<string> executedAssemblies,
        Func<Type, bool> func,
        AssemblyName[] assemblies)
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

                foreach (var type in assembly
                                     .GetTypes()
                                     .Where(
                                         x => x is
                                         {
                                             IsClass: true,
                                             IsAbstract: false,
                                             IsInterface: false,
                                         })
                                     .Where(func))
                {
                    if (results.All(x => x.FullName != type.FullName))
                    {
                        results.Add(type);
                    }
                }

                FindTypes(
                    ref results,
                    ref executedAssemblies,
                    func,
                    assemblies: assembly.GetReferencedAssemblies());
            }
            catch (Exception)
            {
                // If an assembly can't be loaded, we skip it. It hasn't caused issues yet.
            }
        }
    }
}
