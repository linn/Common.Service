namespace Linn.Common.Service
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Linn.Common.Service.Extensions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    public class UniversalResponseNegotiator : IResponseNegotiator
    {
        private readonly IServiceProvider serviceProvider;

        public UniversalResponseNegotiator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public bool CanHandle(MediaTypeHeaderValue accept) => true;

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken)
        {
            var contentType = req.GetContentType();
            var handler = this.serviceProvider.GetHandler(model, contentType);

            if (handler == null)
            {
                res.StatusCode = 406; // Not acceptable
            }
            else
            {
                await handler.Handle(req, res, model, cancellationToken);
            }
        }
    }
}
