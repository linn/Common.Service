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

    public class WhenRequestingResourceAsCsv : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            var widgetResource = new WidgetResource { WidgetName = "Widget 1", Id = 123 };

            this.WidgetService.GetWidget(1).Returns(new SuccessResult<WidgetResource>(widgetResource));

            this.Response = this.Client.Get(
                "/widgets/1",
                with => { with.Accept("text/csv"); }).Result;
        }

        [Test]
        public void ShouldReturnOk()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnCsvContentType()
        {
            var contentTypeHeader = this.Response.Content.Headers.FirstOrDefault(h => h.Key == "Content-Type");
            contentTypeHeader.Should().NotBeNull();
            contentTypeHeader.Value.First().Should().Contain("text/csv");
        }

        [Test]
        public void ShouldReturnCsvBody()
        {
            var res = this.Response.Content;
            var csv = res.ReadAsStringAsync().Result;
            csv.Should().Contain("WidgetName,Id");
            csv.Should().Contain("Widget 1,123");
        }
    }
}
