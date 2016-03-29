using System;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalCustomDictionaryType : LastChangeInfo
    {

        public InternalCustomDictionaryType()
        { }
        public InternalCustomDictionaryType(ModifyCustomDictionaryType model)
        {
            Id = model.Id;
            Code = model.Code;
            Description = model.Description;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код словаря
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Описание словаря
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Значения словаря
        /// </summary>
        public IEnumerable<InternalCustomDictionary> CustomDictionaries { get; set; }
    }
}