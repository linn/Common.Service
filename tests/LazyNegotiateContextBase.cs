namespace Linn.Common.Service.Tests
{
    using Linn.Common.Rendering;
    using Linn.Common.Service.Handlers;
    using Linn.Common.Service.Tests.Fake;
    using Linn.Common.Service.Tests.Fake.Facades;
    using Linn.Common.Service.Tests.Fake.Modules;
    using Linn.Common.Service.Tests.Fake.ResourceBuilders;
    using Linn.Common.Service.Tests.Fake.Resources;

    using Microsoft.Extensions.DependencyInjection;

    using NSubstitute;

    using NUnit.Framework;

    public class LazyNegotiateContextBase
    {
        protected HttpClient Client { get; private set; }

        protected HttpResponseMessage Response { get; set; }

        protected IWidgetService WidgetService { get; private set; }

        protected IViewLoader ViewLoader { get; private set; }

        protected ITemplateEngine TemplateEngine { get; private set; }

        [SetUp]
        public void SetupContext()
        {
            this.WidgetService = Substitute.For<IWidgetService>();
            this.ViewLoader = Substitute.For<IViewLoader>();
            this.TemplateEngine = Substitute.For<ITemplateEngine>();

            this.ViewLoader.Load("Index.cshtml").Returns("<html>@Model.AppSettings</html>");
            this.TemplateEngine.Render(Arg.Any<object>(), Arg.Any<string>())
                .Returns(Task.FromResult("<html>rendered</html>"));

            this.Client = TestClient.With<WidgetModule>(
                s =>
                    {
                        s.AddSingleton<WidgetResourceBuilder>();
                        s.AddSingleton(this.ViewLoader);
                        s.AddSingleton(this.TemplateEngine);
                        s.AddSingleton<IResponseNegotiator, HtmlNegotiator>();
                        s.AddTransient<UniversalResponseNegotiator>();
                        s.AddSingleton<IHandler, JsonResultHandler<WidgetResource>>();
                        s.AddSingleton<IHandler, CsvResultHandler<WidgetResource>>();
                        s.AddSingleton(this.WidgetService);
                    },
                FakeAuthMiddleware.EmployeeMiddleware);
        }
    }
}
