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

        public InternalCustomDictionaryType(AddCustomDictionaryType model)
        {
            SetInternalCustomDictionaryType(model);
        }

        public InternalCustomDictionaryType(ModifyCustomDictionaryType model)
        {
            Id = model.Id;
            SetInternalCustomDictionaryType(model);
        }

        private void SetInternalCustomDictionaryType(AddCustomDictionaryType model)
        {
            Code = model.Code;
            Name = model.Name;
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
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание словаря
        /// </summary>
        public string Description { get; set; }

    }
}