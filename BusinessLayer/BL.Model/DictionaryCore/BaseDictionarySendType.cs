using System;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionarySendType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsImportant { get; set; }
        public int SubordinationTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string SubordinationTypeName { get; set; }
    }
}