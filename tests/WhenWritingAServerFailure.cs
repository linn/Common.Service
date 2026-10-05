namespace Linn.Common.Service.Tests
{
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.Common.Facade;
    using Linn.Common.Service.Handlers;
    using Linn.Common.Service.Tests.Fake.Resources;

    using Microsoft.AspNetCore.Http;

    using NUnit.Framework;

    public class WhenWritingAServerFailure
    {
        private JsonResultHandler<WidgetResource> handler;

        private DefaultHttpContext context;

        [SetUp]
        public void SetUp()
        {
            this.handler = new JsonResultHandler<WidgetResource>();
            this.context = new DefaultHttpContext();
            this.context.Response.Body = new MemoryStream();
        }

        [Test]
        public async Task ShouldSendTheUserMessageButNotTheDetail()
        {
            await this.Write(new ServerFailureResult<WidgetResource>(
                "The change was saved, but writing its log failed (SqlException: deadlock on log_table)",
                "The change was saved, but writing its log failed"));

            this.context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            (await this.Body()).Should().Be("\"The change was saved, but writing its log failed\"");
        }

        [Test]
        public async Task ShouldSendNothingForAnExistingServerFailure()
        {
            // a message only - diagnostic, e.g. an upstream service's raw error - stays unsent
            await this.Write(new ServerFailureResult<WidgetResource>("Unexpected status code 502: upstream stack trace"));

            this.context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            (await this.Body()).Should().BeEmpty();
        }

        private Task Write(IResult<WidgetResource> result) =>
            this.handler.Handle(this.context.Request, this.context.Response, result, CancellationToken.None);

        private async Task<string> Body()
        {
            this.context.Response.Body.Position = 0;
            return await new StreamReader(this.context.Response.Body).ReadToEndAsync();
        }
    }
}
