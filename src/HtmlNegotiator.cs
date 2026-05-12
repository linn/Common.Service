namespace Linn.Common.Service
{
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Configuration;
    using Linn.Common.Rendering;
    using Linn.Common.Service.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class HtmlNegotiator : IResponseNegotiator
    {
        private readonly IViewLoader viewLoader;

        private readonly ITemplateEngine templateEngine;

        private readonly HtmlNegotiatorOptions options;

        public HtmlNegotiator(IViewLoader viewLoader, ITemplateEngine templateEngine)
            : this(viewLoader, templateEngine, new HtmlNegotiatorOptions())
        {
        }

        public HtmlNegotiator(
            IViewLoader viewLoader,
            ITemplateEngine templateEngine,
            HtmlNegotiatorOptions options)
        {
            this.viewLoader = viewLoader;
            this.templateEngine = templateEngine;
            this.options = options;
        }

        public bool CanHandle(MediaTypeHeaderValue accept)
        {
            return accept.MediaType.Equals("text/html");
        }

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken)
        {
            var viewName = model is ViewResponse viewResponse
                ? viewResponse.ViewName
                : "Index.cshtml";

            var view = this.viewLoader.Load(viewName);

            var appSettings = ApplicationSettings.GetDefaults();
            foreach (var kvp in this.options.ExtraSettings)
            {
                appSettings.Settings[kvp.Key] = kvp.Value;
            }

            var jsonAppSettings = JsonConvert.SerializeObject(
                appSettings.Settings,
                Formatting.Indented,
                new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

            var viewModel = new ViewModel
                                {
                                    AppSettings = jsonAppSettings,
                                    BuildNumber = ConfigurationManager.Configuration["BUILD_NUMBER"]
                                };

            var compiled = this.templateEngine.Render(viewModel, view).Result;

            res.ContentType = "text/html";
            res.StatusCode = (int)HttpStatusCode.OK;

            await res.WriteAsync(compiled, cancellationToken);
        }
    }
}
