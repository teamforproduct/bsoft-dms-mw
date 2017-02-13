using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// "Настройка правил рассылки между должностями (для исполнения, для сведения)", добавления/редактирования записи.
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class SetJournalAccessByDepartment_Position
    {

        /// <summary>
        /// Id Должности
        /// </summary>
        [Required]
        public int PositionId { get; set; }

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