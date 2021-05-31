using System.Linq;

namespace forest_report_api.Models
{
    public class TableItem
    {
        public string Title { get; set; }
        public int? TitlePosition { get; set; }
        public string CodeItem { get; set; }
        public string CodeParent { get; set; }
    }
}