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

    public class WhenRequestingAResourceThatCannotBeFound : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            this.WidgetService.GetWidget(999).Returns(new NotFoundResult<WidgetResource>());

            this.Client.Accept("application/json");
            this.Response = this.Client.Get(
                "widgets/999",
                with => { with.Accept("application/json"); }).Result;
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public void ShouldReturnJsonContentType()
        {
            var contentTypeHeader = this.Response.Content.Headers.FirstOrDefault(h => h.Key == "Content-Type");
            contentTypeHeader.Should().NotBeNull();
            contentTypeHeader.Value.First().Should().Contain("application/json");
        }
    }
}
