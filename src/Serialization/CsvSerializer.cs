namespace Linn.Common.Service.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using CsvHelper;

    using Linn.Common.Reporting.Resources.Extensions;
    using Linn.Common.Reporting.Resources.ReportResultResources;

    public class CsvSerializer : ISerializer
    {
        public string Serialize(object model)
        {
            var sw = new StringWriter();

            var writer = new CsvWriter(sw, CultureInfo.InvariantCulture);

            if (model is ReportReturnResource res)
            {
                model = res.ConvertToCsvList();
            }

            if (model is IEnumerable<IEnumerable> csvGrid)
            {
                foreach (var line in csvGrid)
                {
                    foreach (var field in line)
                    {
                        writer.WriteField(field);
                    }

                    writer.NextRecord();
                }
            }
            else if (model is IEnumerable arrayModel)
            {
                writer.WriteRecords(arrayModel);
            }
            else
            {
                writer.WriteRecords(new[] { model });
            }

            writer.Flush();

            return sw.ToString();
        }

        public T Deserialize<T>(string body)
        {
            throw new NotImplementedException();
        }
    }
}
