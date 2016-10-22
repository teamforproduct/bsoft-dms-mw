using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования отдела в штатном расписании
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryDepartment
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
        /// Вышестоящее подразделение
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Полное наименование подразделения
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// Индекс подразделения
        /// </summary>
        [Required]
        public string Index { get; set; }

        /// <summary>
        /// Компания
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        [IgnoreDataMember]
        public int? ChiefPositionId { get; set; }

    }
}
