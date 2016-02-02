using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryLinkType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsImpotant { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}