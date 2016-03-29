using System;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// адрес контрагента
    /// </summary>
    public class InternalDictionaryAgentAddress : LastChangeInfo
    {

        public InternalDictionaryAgentAddress()
        { }

        public InternalDictionaryAgentAddress(ModifyDictionaryAgentAddress Model)
        {
            Id = Model.Id;
            AgentId = Model.AgentId;
            AddressTypeID = Model.AddressTypeId;
            PostCode = Model.PostCode;
            Address = Model.Address;
            IsActive = Model.IsActive;
            Description = Model.Description;
        }
        
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ссылка на контрагента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// ссылка на тип адреса
        /// </summary>
        public int AddressTypeID { get; set; }
        /// <summary>
        /// индекс
        /// </summary>
        public string PostCode { get; set; }
        /// <summary>
        /// адрес
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// комментарии
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}
