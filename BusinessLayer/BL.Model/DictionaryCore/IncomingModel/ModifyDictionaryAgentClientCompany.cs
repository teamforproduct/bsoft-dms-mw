using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Компании"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryAgentClientCompany
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Признак активности.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Наименование компании
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Наименование компании
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Description { get; set; }
    }
}
