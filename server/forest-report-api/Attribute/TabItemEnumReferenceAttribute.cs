using forest_report_api.Models;

namespace forest_report_api.Attribute
{
    public class TabItemEnumReferenceAttribute : System.Attribute
    {
        public TabItemEnumReferenceAttribute(TabName tabName)
        {
            TabNameProperty = tabName;
        }

        public TabName TabNameProperty { get; set; }
    }
}