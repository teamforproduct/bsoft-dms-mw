using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Model.FullTextSearch
{
    public class FilterFullTextIndexCash
    {
        public int ClientId { get; set; }
        public int? IdFrom { get; set; }
        public int? IdTo { get; set; }
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public int OperationType { get; set; }
    }
}