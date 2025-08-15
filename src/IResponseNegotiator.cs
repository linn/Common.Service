namespace Linn.Common.Service
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    public interface IResponseNegotiator
    {
        bool CanHandle(MediaTypeHeaderValue accept);

        Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken);
    }
}
