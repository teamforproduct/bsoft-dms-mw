using System;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryDocumentTypes
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
