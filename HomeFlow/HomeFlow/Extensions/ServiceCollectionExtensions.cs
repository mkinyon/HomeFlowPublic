
using System.Reflection;
using HomeFlow.Services;

namespace HomeFlow.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAllFeatureServices( this IServiceCollection services, Assembly assembly )
    {
        var allTypes = assembly.GetTypes();
        var serviceTypes = allTypes.Where( t => t.IsClass && !t.IsAbstract && t.Name.EndsWith( "Service" ) ).ToList();
        
        foreach ( var implType in serviceTypes )
        {
            RegisterService( services, implType );
        }
    }

    public static IServiceCollection AddDynamicServices( this IServiceCollection services, params string[] assemblyNames )
    {
        // Get all loaded assemblies in the current AppDomain
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where( a => assemblyNames.Any( name => a.GetName().Name != null && a.GetName().Name!.Contains( name, StringComparison.OrdinalIgnoreCase ) ) )
            .ToArray();

        foreach ( var assembly in assemblies )
        {
            // Register FluentValidation Validators
            var validatorTypes = assembly.GetTypes()
                .Where( t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( IValidator<> ) ) );

            foreach ( var validatorType in validatorTypes )
            {
                var validatorInterface = validatorType.GetInterfaces()
                    .First( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( IValidator<> ) );
                services.AddScoped( validatorInterface, validatorType );
            }

            // Register Services
            var serviceTypes = assembly.GetTypes()
                .Where( t => t.IsClass && !t.IsAbstract && t.Name.EndsWith( "Service" ) );

            foreach ( var serviceType in serviceTypes )
            {
                RegisterService( services, serviceType );
            }
        }

        return services;
    }

    /// <summary>
    /// Registers a service with all its interfaces
    /// </summary>
    private static void RegisterService( IServiceCollection services, Type serviceType )
    {
        var interfaces = serviceType.GetInterfaces();

        // 1. Register IService<T> if implemented
        var genericServiceInterface = interfaces
            .FirstOrDefault( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( IService<> ) );

        if ( genericServiceInterface != null )
        {
            services.AddScoped( genericServiceInterface, serviceType );
        }

        // 2. Register I{Name}Service if implemented (exact match)
        var namedInterface = interfaces
            .FirstOrDefault( i => i.Name == $"I{serviceType.Name}" );

        if ( namedInterface != null )
        {
            services.AddScoped( namedInterface, serviceType );
        }

        // 3. Register concrete type if no interfaces found
        if ( !interfaces.Any() )
        {
            services.AddScoped( serviceType );
        }
    }

    /// <summary>
    /// Registers services from multiple assemblies by loading them explicitly
    /// </summary>
    public static IServiceCollection AddServicesFromAssemblies( this IServiceCollection services, params string[] assemblyNames )
    {
        foreach ( var assemblyName in assemblyNames )
        {
            try
            {
                var assembly = Assembly.Load( assemblyName );
                var serviceTypes = assembly.GetTypes()
                    .Where( t => t.IsClass && !t.IsAbstract && t.Name.EndsWith( "Service" ) );

                foreach ( var serviceType in serviceTypes )
                {
                    RegisterService( services, serviceType );
                }

                // Also register validators
                var validatorTypes = assembly.GetTypes()
                    .Where( t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                        .Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( IValidator<> ) ) );

                foreach ( var validatorType in validatorTypes )
                {
                    var validatorInterface = validatorType.GetInterfaces()
                        .First( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( IValidator<> ) );
                    services.AddScoped( validatorInterface, validatorType );
                }
            }
            catch ( Exception ex )
            {
                // Log the error but don't fail the application startup
                Console.WriteLine( $"Warning: Could not load assembly {assemblyName}: {ex.Message}" );
            }
        }

        return services;
    }
}
