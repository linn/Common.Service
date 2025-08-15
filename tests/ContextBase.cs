namespace Linn.Common.Service.Tests
{
    using Linn.Common.Service.Handlers;
    using Linn.Common.Service.Tests.Fake;
    using Linn.Common.Service.Tests.Fake.Facades;
    using Linn.Common.Service.Tests.Fake.Modules;
    using Linn.Common.Service.Tests.Fake.ResourceBuilders;
    using Linn.Common.Service.Tests.Fake.Resources;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    public class ContextBase
    {
        protected HttpClient Client { get; private set; }

        protected HttpResponseMessage Response { get; set; }

        protected IWidgetService WidgetService { get; private set; }

        [SetUp]
        public void SetupContext()
        {
            this.WidgetService = NSubstitute.Substitute.For<IWidgetService>();

            this.Client = TestClient.With<WidgetModule>(
                s =>
                    {
                        s.AddSingleton<WidgetResourceBuilder>();
                        s.AddTransient<UniversalResponseNegotiator>();
                        s.AddSingleton<IHandler, JsonResultHandler<WidgetResource>>();
                        s.AddSingleton<IHandler, CsvResultHandler<WidgetResource>>();
                        s.AddSingleton(this.WidgetService);
                    },
                FakeAuthMiddleware.EmployeeMiddleware);
        }
    }
}
