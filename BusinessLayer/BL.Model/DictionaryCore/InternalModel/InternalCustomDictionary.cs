using System;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalCustomDictionary : LastChangeInfo
    {
        public InternalCustomDictionary()
        { }

        public InternalCustomDictionary(ModifyCustomDictionary model)
        {
            Id = model.Id; 
            Code = model.Code;
            Description = model.Description;
            DictionaryTypeId = model.DictionaryTypeId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ид. словаря
        /// </summary>
        public int DictionaryTypeId { get; set; }
        /// <summary>
        /// Код значения словаря
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Описание значения словаря
        /// </summary>
        public string Description { get; set; }
    }
}