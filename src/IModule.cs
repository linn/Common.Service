namespace Linn.Common.Service
{
    using Microsoft.AspNetCore.Routing;

    public interface IModule
    {
        void MapEndpoints(IEndpointRouteBuilder endpoints);
    }
}
