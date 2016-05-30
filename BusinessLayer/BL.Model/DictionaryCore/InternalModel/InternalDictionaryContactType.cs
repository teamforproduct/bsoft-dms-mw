using BL.Model.Common;
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

        public InternalDictionaryContactType(ModifyDictionaryContactType Model)
        {
            Id = Model.Id;
            Name = Model.Name;
            InputMask = Model.InputMask;
            Code = Model.Code;
            IsActive = Model.IsActive;
        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
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
