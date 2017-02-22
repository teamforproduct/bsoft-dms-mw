using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.System
{
    public class FullTextIndexCash
    {
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        public int Id { get; set; }
        public int ObjectId { get; set; }
        public int ObjectType { get; set; }
        public int OperationType { get; set; }
    }
}