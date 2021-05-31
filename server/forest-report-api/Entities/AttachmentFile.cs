using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace forest_report_api.Entities
{
    public class AttachmentFile : BaseEntity
    {
        public string Type { get; set; }
        public byte[] Value { get; set; }
        public string Name { get; set; }
    }
}