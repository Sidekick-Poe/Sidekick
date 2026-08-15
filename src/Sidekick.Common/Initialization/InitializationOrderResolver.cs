using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Initialization;

public class InitializationOrderResolver(IServiceProvider serviceProvider)
{

    /// <summary>
    /// Resolves initialization order based on constructor dependencies using topological sort.
    /// </summary>
    public List<IInitializableService> GetOrderedServices(IEnumerable<Type> serviceTypes)
    {
        var services = new Dictionary<Type, IInitializableService>();
        var dependencies = new Dictionary<Type, HashSet<Type>>();

        // Build the dependency graph
        var allServiceTypes = serviceTypes.ToList();
        foreach (var serviceType in allServiceTypes)
        {
            var service = (IInitializableService)serviceProvider.GetRequiredService(serviceType);
            services[serviceType] = service;
            dependencies[serviceType] = GetDependencies(serviceType, allServiceTypes);
        }

        // Perform topological sort
        var sorted = new List<IInitializableService>();
        var visited = new HashSet<Type>();
        var visiting = new HashSet<Type>();

        foreach (var serviceType in services.Keys)
        {
            Visit(serviceType, services, dependencies, visited, visiting, sorted);
        }

        return sorted;
    }

    private HashSet<Type> GetDependencies(Type serviceType, IEnumerable<Type> allServiceTypes)
    {
        var deps = new HashSet<Type>();
        var allServiceTypeSet = new HashSet<Type>(allServiceTypes);

        // Get all constructors (usually there's only one)
        var constructors = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();

            foreach (var parameter in parameters)
            {
                var paramType = parameter.ParameterType;

                // Check if this parameter is one of our initializable services
                if (allServiceTypeSet.Contains(paramType))
                {
                    deps.Add(paramType);
                }
                else
                {
                    // Check if it's an interface implemented by one of our services
                    var implementingService = allServiceTypeSet.FirstOrDefault(st =>
                        paramType.IsAssignableFrom(st));

                    if (implementingService != null)
                    {
                        deps.Add(implementingService);
                    }
                }
            }
        }

        return deps;
    }

    private void Visit(
        Type serviceType,
        Dictionary<Type, IInitializableService> services,
        Dictionary<Type, HashSet<Type>> dependencies,
        HashSet<Type> visited,
        HashSet<Type> visiting,
        List<IInitializableService> sorted)
    {
        if (visited.Contains(serviceType))
        {
            return;
        }

        if (visiting.Contains(serviceType))
        {
            throw new InvalidOperationException(
                $"Circular dependency detected involving {serviceType.Name}");
        }

        visiting.Add(serviceType);

        foreach (var dependency in dependencies[serviceType])
        {
            Visit(dependency, services, dependencies, visited, visiting, sorted);
        }

        visiting.Remove(serviceType);
        visited.Add(serviceType);
        sorted.Add(services[serviceType]);
    }
}