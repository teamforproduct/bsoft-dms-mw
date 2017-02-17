using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalAgentFavourite : LastChangeInfo
    {
        public InternalAgentFavourite()
        { }

        public InternalAgentFavourite(AddAgentFavourite model)
        {
            SetInternal(model);
        }

        private void  SetInternal(AddAgentFavourite model)
        {
            ObjectId = model.ObjectId;
            Module = model.Module;
            Feature = model.Feauture;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ид. объекта
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Модуль
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// Фича
        /// </summary>
        public string Feature { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Ид. объекта
        /// </summary>
        public int AgentId { get; set; }
    }
}