using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryResultType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsExecute { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

    }
}