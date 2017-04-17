using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/редактирования записи справочника "Штатное расписание"
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class AddPosition
    {

        /// <summary>
        /// Признак активности.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Вышестоящая должность
        /// </summary>
        [IgnoreDataMember]
        public int? ParentId { get; set; }


        /// <summary>
        /// Наименование должности
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Полное наименование должности
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// Подразделение, в которое включена должность
        /// </summary>
        [Required]
        public int DepartmentId { get; set; }

        /// <summary>
        /// Порядковый номер (значимость) должности в подразделении
        /// </summary>
        [Required]
        public int Order { get; set; }

        [IgnoreDataMember]
        public Roles? Role { get; set; }

    }
}
