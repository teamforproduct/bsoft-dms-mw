using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.IncomingModel
{ 
    public class SetJournalAccessByDepartment_Journal
    {

        /// <summary>
        /// Журнал
        /// </summary>
        [Required]
        public int JournalId { get; set; }

        /// <summary>
        /// Подразделение
        /// </summary>
        [Required]
        public int DepartmentId { get; set; }

        /// <summary>
        /// Тип доступа (для просмотра, для регистрации)
        /// </summary>
        [Required]
        public EnumRegistrationJournalAccessTypes RegJournalAccessTypeId { get; set; }

        /// <summary>
        /// Установить галочку
        /// </summary>
        [Required]
        public bool IsChecked { get; set; }

        /// <summary>
        /// Не применять к дочерним отделам
        /// </summary>
        public bool IgnoreChildDepartments { get; set; }

    }
}