using System;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryDocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DirectionCodes { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

    }
}