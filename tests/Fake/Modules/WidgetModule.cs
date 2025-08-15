namespace Linn.Common.Service.Tests.Fake.Modules
{
    using System.Threading.Tasks;

    using Linn.Common.Service.Extensions;
    using Linn.Common.Service.Tests.Fake.Facades;
    using Linn.Common.Service.Tests.Fake.Resources;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    public class WidgetModule : IModule
    {
        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/widgets/{id:int}", this.GetWidget);
            app.MapPost("/widgets", this.PostWidget);
        }

        private async Task GetWidget(
            HttpRequest req, HttpResponse res, int id, IWidgetService widgetService)
        {
            var result = widgetService.GetWidget(id);

            await res.Negotiate(result);
        }

        private async Task PostWidget(HttpRequest req, HttpResponse res, WidgetResource resource, IWidgetService widgetService)
        {
            var result = widgetService.CreateWidget(resource);

            await res.Negotiate(result);
        }
    }
}
