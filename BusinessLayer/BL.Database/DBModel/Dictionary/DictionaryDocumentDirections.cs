using System;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryDocumentDirections
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
