using System;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryTag : ModifyDictionaryTag
    {
        /// <summary>
        /// ИД. должности
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// ИД. пользователя, изменившего запись
        /// </summary>
        public int LastChangeUserId { get; set; }

        /// <summary>
        /// Дата последнего изменения записи
        /// </summary>
        public DateTime LastChangeDate { get; set; }
    }
}