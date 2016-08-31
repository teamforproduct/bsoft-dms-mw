using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryAddressType : LastChangeInfo
    {

        public InternalDictionaryAddressType()
        { }

        public InternalDictionaryAddressType(ModifyDictionaryAddressType model)
        {
            Id = model.Id;
            Code = model.Code;
            Name = model.Name;
            IsActive = model.IsActive;
        }
        
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Краткое наименование типа адреса.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Название типа адреса.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}