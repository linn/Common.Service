namespace Linn.Common.Service.Tests.Fake.Facades
{
    using Linn.Common.Facade;
    using Linn.Common.Service.Tests.Fake.Resources;

    public interface IWidgetService
    {
        IResult<WidgetResource> GetWidget(int widgetId);

        IResult<WidgetResource> CreateWidget(WidgetResource resource);
    }
}
