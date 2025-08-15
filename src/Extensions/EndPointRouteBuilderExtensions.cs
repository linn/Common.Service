namespace Linn.Common.Service.Extensions
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Routing;

    public static class EndPointRouteBuilderExtensions
    {
        public static void MapEndpoints(this IEndpointRouteBuilder app)
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies().SelectMany(a => a.GetTypes());

            var modules = types.Where(p => ImplementsInterface(p, typeof(IModule)))
                .Select(Activator.CreateInstance)
                .Cast<IModule>();

            foreach (var module in modules)
            {
                module.MapEndpoints(app);
            }
        }

        private static bool ImplementsInterface(Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            return interfaces.Any(i => i == interfaceType);
        }
    }
}
