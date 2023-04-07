using System.Reflection;

namespace CatalogService.API.Core;

public static class EndpointDefinitionExtensions
{
    public static void AddAllEndpointDefinitions(this IServiceCollection services)
    {
        var endpointsDefinitions = new List<IEndpointDefinition>();

        endpointsDefinitions.AddRange(Assembly.GetCallingAssembly().ExportedTypes
            .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IEndpointDefinition>());


        foreach (var endpoint in endpointsDefinitions)
        {
            endpoint.DefineServices(services);
        }

        services.AddSingleton(endpointsDefinitions as IReadOnlyCollection<IEndpointDefinition>);
    }

    public static void UseEndpointDefinitions(this WebApplication app)
    {
        var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();
        foreach (var endpointDefinition in definitions)
        {
            endpointDefinition.DefineEndpoints(app);
        }
    }
}
