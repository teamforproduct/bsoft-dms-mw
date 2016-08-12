using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    public class ModifyDictionaryTag: CurrentPosition
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

    }
}
