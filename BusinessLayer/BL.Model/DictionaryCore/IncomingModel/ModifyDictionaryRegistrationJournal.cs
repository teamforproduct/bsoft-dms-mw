using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Журнал регистрации"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryRegistrationJournal
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
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
