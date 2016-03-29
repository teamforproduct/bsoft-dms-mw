using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Журналы регистрации"
    /// </summary>
    public class InternalDictionaryRegistrationJournal : LastChangeInfo
    {

        public InternalDictionaryRegistrationJournal()
        {
        }

        public InternalDictionaryRegistrationJournal(ModifyDictionaryRegistrationJournal model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            Name = model.Name;
            DepartmentId = model.DepartmentId;
            Index = model.Index;
            IsIncoming = model.IsIncoming;
            IsOutcoming = model.IsOutcoming;
            IsInternal = model.IsInternal;
            PrefixFormula = model.PrefixFormula;
            NumerationPrefixFormula = model.NumerationPrefixFormula;
            SuffixFormula = model.SuffixFormula;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Признак активности.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Название (Заголовок) журнала регистрацииа.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Подразделение, к которому приписан журнал регистрации
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Индекс журнала
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Журнал может содержать входящие(Incoming) документы
        /// </summary>
        public bool IsIncoming { get; set; }

        /// <summary>
        /// Журнал может содержать исходящие(Outcoming) документы
        /// </summary>
        public bool IsOutcoming { get; set; }

        /// <summary>
        /// Журнал может содержать внутренние(Internal) документы
        /// </summary>
        public bool IsInternal { get; set; }

        /// <summary>
        /// Выражение, описывающие формирование префикса номера документа, в этом журнале регистрации 
        /// </summary>
        public string PrefixFormula { get; set; }

        /// <summary>
        /// Выражение, описывающие формирование порядкового номера документа, в этом журнале регистрации 
        /// </summary>
        public string NumerationPrefixFormula { get; set; }

        /// <summary>
        /// Выражение, описывающие формирование суффикса номера документа, в этом журнале регистрации 
        /// </summary>
        public string SuffixFormula { get; set; }

        // !!! После добавления полей внеси изменения в BL.Logic.Common.CommonDictionaryUtilities.RegistrationJournalModifyToInternal

    }
}