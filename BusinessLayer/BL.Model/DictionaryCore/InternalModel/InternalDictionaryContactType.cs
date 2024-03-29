﻿using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// тип контакта
    /// </summary>
    public class InternalDictionaryContactType : LastChangeInfo
    {

        public InternalDictionaryContactType()
        { }

        public InternalDictionaryContactType(AddContactType Model)
        {
            SetInternalDictionaryContactType(Model);
        }

        public InternalDictionaryContactType(ModifyContactType Model)
        {
            Id = Model.Id;
            SetInternalDictionaryContactType(Model);
        }

        private void SetInternalDictionaryContactType(AddContactType Model)
        {
            Name = Model.Name;
            InputMask = Model.InputMask;
            Code = Model.Code;
            SpecCode = Model.SpecCode;
            IsActive = Model.IsActive;
        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Внутренний код
        /// </summary>
        public string SpecCode { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Маска для ввода
        /// </summary>
        public string InputMask { get; set; }

        public string Code { get; set; }

        /// <summary>
        /// признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}
