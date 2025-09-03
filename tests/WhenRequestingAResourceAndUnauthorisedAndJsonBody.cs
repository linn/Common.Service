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

    public class WhenRequestingAResourceAndUnauthorisedAndJsonBody : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            this.WidgetService.GetWidget(999).Returns(new UnauthorisedResult<WidgetResource>(new ErrorResource { ErrorMessage = "An error occurred"}));

            this.Client.Accept("application/json");
            this.Response = this.Client.Get(
                "widgets/999",
                with => { with.Accept("application/json"); }).Result;
        }

        [Test]
        public void ShouldReturn401()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Test]
        public void ShouldReturnJsonBody()
        {
            var result = this.Response.DeserializeBody<ErrorResource>();
            result.ErrorMessage.Should().Be("An error occurred");
        }
    }
}
