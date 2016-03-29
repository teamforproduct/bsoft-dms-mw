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
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        /// <summary>
        /// ИД. должности
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// ИД. пользователя, изменившего запись
        /// </summary>
        //public int LastChangeUserId { get; set; }

        /// <summary>
        /// Дата последнего изменения записи
        /// </summary>
        //public DateTime LastChangeDate { get; set; }
    }
}