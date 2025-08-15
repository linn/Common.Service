namespace Linn.Common.Service.Tests.Fake.ResourceBuilders
{
    using System;

    using Linn.Common.Facade;
    using Linn.Common.Service.Tests.Fake.Domain;
    using Linn.Common.Service.Tests.Fake.Resources;

    public class WidgetResourceBuilder : IResourceBuilder<Widget>
    {
        public object Build(Widget widget)
        {
            return new WidgetResource
            {
                WidgetName = widget.Name
            };
        }

        public string GetLocation(Widget model)
        {
            throw new NotImplementedException();
        }
    }
}
