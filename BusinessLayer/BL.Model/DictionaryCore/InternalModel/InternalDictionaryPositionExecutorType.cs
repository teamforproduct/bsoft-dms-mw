using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Штатное расписание"
    /// </summary>
    public class InternalDictionaryPositionExecutorType : LastChangeInfo
    {

        public InternalDictionaryPositionExecutorType()
        { }

        public InternalDictionaryPositionExecutorType(ModifyDictionaryPositionExecutorType model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            Code = model.Code;
            Name = model.Name;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Наименование типа исполнения
        /// </summary>
        public string Name { get; set; }

    }
}