using System;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryTag : LastChangeInfo//ModifyDictionaryTag
    {

        public InternalDictionaryTag()
        { }

        public InternalDictionaryTag(ModifyDictionaryTag Model)
        {
            Id = Model.Id;
            Name = Model.Name;
            Color = Model.Color;
            IsActive = Model.IsActive;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public int? PositionId { get; set; }
        
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        
        public DateTime LastChangeDate { get; set; }
    }
}