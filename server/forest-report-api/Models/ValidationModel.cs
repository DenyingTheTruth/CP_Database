using System.Collections.Generic;

namespace forest_report_api.Models
{
    public class ValidationModel
    {
        public int TabIndex { get; set; }
        public List<ValidationItem> Revisions { get; set; }
    }

    public class ValidationItem
    {
        public string CodeItem { get; set; }
        public string Cell { get; set; }
    }
}