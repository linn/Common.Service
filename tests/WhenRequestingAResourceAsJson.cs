namespace Linn.Common.Service.Tests
{
    using System.Linq;
    using System.Net;

    using FluentAssertions;

    using Linn.Common.Facade;
    using Linn.Common.Service.Tests.Extensions;
    using Linn.Common.Service.Tests.Fake.Resources;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenRequestingAResourceAsJson : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            var widgetResource = new WidgetResource { WidgetName = "Widget 1" };

            this.WidgetService.GetWidget(1).Returns(new SuccessResult<WidgetResource>(widgetResource));

            this.Response = this.Client.Get(
                "/widgets/1",
                with => { with.Accept("application/json"); }).Result;
        }

        [Test]
        public void ShouldReturnOk()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnJsonContentType()
        {
            var contentTypeHeader = this.Response.Content.Headers.FirstOrDefault(h => h.Key == "Content-Type");
            contentTypeHeader.Should().NotBeNull();
            contentTypeHeader.Value.First().Should().Contain("application/json");
        }

        [Test]
        public void ShouldReturnJsonBody()
        {
            var resources = this.Response.DeserializeBody<WidgetResource>();
            resources.Should().NotBeNull();

            resources.WidgetName.Should().Be("Widget 1");
        }
    }
}
