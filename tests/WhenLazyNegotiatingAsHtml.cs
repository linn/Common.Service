namespace Linn.Common.Service.Tests
{
    using System.Net;

    using FluentAssertions;

    using Linn.Common.Service.Tests.Extensions;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenLazyNegotiatingAsHtml : LazyNegotiateContextBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Response = this.Client.Get(
                "/widgets/1/lazy",
                with => { with.Accept("text/html"); }).Result;
        }

        [Test]
        public void ShouldReturnOk()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldReturnHtmlContentType()
        {
            this.Response.Content.Headers.ContentType.MediaType.Should().Be("text/html");
        }

        [Test]
        public void ShouldReturnRenderedHtml()
        {
            var body = this.Response.Content.ReadAsStringAsync().Result;
            body.Should().Be("<html>rendered</html>");
        }

        [Test]
        public void ShouldNotHaveInvokedTheService()
        {
            this.WidgetService.DidNotReceive().GetWidget(Arg.Any<int>());
        }
    }
}
