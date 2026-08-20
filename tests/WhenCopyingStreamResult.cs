namespace Linn.Common.Service.Tests
{
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Linn.Common.Facade;
    using Linn.Common.Service.Handlers;

    using Microsoft.AspNetCore.Http;

    using NUnit.Framework;

    public class WhenCopyingStreamResult
    {
        private StreamCopyingResultHandler handler;

        private DefaultHttpContext context;

        [SetUp]
        public void SetUp()
        {
            this.handler = new StreamCopyingResultHandler();
            this.context = new DefaultHttpContext();
            this.context.Response.Body = new MemoryStream();
        }

        [Test]
        public async Task ShouldCopySuccessStreamAndSetAttachmentDisposition()
        {
            var payload = Encoding.UTF8.GetBytes("zip-bytes");
            var result = new SuccessResult<StreamResponse>(
                new StreamResponse
                {
                    Stream = new MemoryStream(payload),
                    ContentType = "application/zip",
                    FileName = "linn-resources.zip",
                    Disposition = "attachment"
                });

            await this.handler.Handle(this.context.Request, this.context.Response, result, CancellationToken.None);

            this.context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            this.context.Response.ContentType.Should().Be("application/zip");
            this.context.Response.Headers["Content-Disposition"].ToString()
                .Should().Be("attachment; filename=\"linn-resources.zip\"");

            this.context.Response.Body.Position = 0;
            using var reader = new StreamReader(this.context.Response.Body);
            (await reader.ReadToEndAsync()).Should().Be("zip-bytes");
        }

        [Test]
        public async Task ShouldDefaultToInlineDisposition()
        {
            var result = new SuccessResult<StreamResponse>(
                new StreamResponse
                {
                    Stream = new MemoryStream(Encoding.UTF8.GetBytes("pdf")),
                    ContentType = "application/pdf",
                    FileName = "invoice.pdf"
                });

            await this.handler.Handle(this.context.Request, this.context.Response, result, CancellationToken.None);

            this.context.Response.Headers["Content-Disposition"].ToString()
                .Should().Be("inline; filename=\"invoice.pdf\"");
        }

        [Test]
        public async Task ShouldReturn403ForForbiddenResult()
        {
            var result = new ForbiddenResult<StreamResponse>("Access denied");

            await this.handler.Handle(this.context.Request, this.context.Response, result, CancellationToken.None);

            this.context.Response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);

            this.context.Response.Body.Position = 0;
            using var reader = new StreamReader(this.context.Response.Body);
            (await reader.ReadToEndAsync()).Should().Be("Access denied");
        }

        [Test]
        public async Task ShouldReturn404ForNotFoundResult()
        {
            var result = new NotFoundResult<StreamResponse>("nope");

            await this.handler.Handle(this.context.Request, this.context.Response, result, CancellationToken.None);

            this.context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
