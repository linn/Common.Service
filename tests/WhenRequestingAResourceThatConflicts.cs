namespace Linn.Common.Service.Tests
{
    using System.Net;

    using FluentAssertions;

    using Linn.Common.Facade;
    using Linn.Common.Service.Tests.Extensions;
    using Linn.Common.Service.Tests.Fake.Resources;

    using NSubstitute;

    using NUnit.Framework;

    public class WhenRequestingAResourceThatConflicts : ContextBase
    {
        [SetUp]
        public void SetUp()
        {
            this.WidgetService.GetWidget(999).Returns(
                new ConflictResult<WidgetResource>("This widget has changed since you opened it"));

            this.Client.Accept("application/json");
            this.Response = this.Client.Get(
                "widgets/999",
                with => { with.Accept("application/json"); }).Result;
        }

        [Test]
        public void ShouldReturnConflict()
        {
            this.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Test]
        public void ShouldReturnTheMessage()
        {
            this.Response.DeserializeBody<string>().Should().Be(
                "This widget has changed since you opened it");
        }
    }
}
