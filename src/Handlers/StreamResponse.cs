namespace Linn.Common.Service.Handlers
{
    public class StreamResponse
    {
        public Stream Stream { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        // inline (preview, e.g. PDFs) or attachment (force download, e.g. zips)
        public string Disposition { get; set; } = "inline";
    }
}
