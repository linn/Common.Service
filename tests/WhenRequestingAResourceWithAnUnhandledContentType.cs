namespace Linn.Common.Service.Tests
{
    using System.Net;

    using FluentAssertions;

    using Linn.Common.Facade;
    using Linn.Common.Service.Tests.Extensions;
    using Linn.Common.Service.Tests.Fake.Resources;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenRequestingAResourceWithAnUnhandledContentType : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            var widget = new WidgetResource { WidgetName = "Widget 1" };

            this.WidgetService.GetWidget(1).Returns(new SuccessResult<WidgetResource>(widget));

            this.Client.Accept("application/xml");
            this.Response = this.Client.GetAsync("/widgets/1").Result;
        }

        [Test]
        public void ShouldReturnNotAcceptable()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
        }
    }
}
