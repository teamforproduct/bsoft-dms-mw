using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string Color { get; set; }
        public string PositionName { get; set; }
    }
}
