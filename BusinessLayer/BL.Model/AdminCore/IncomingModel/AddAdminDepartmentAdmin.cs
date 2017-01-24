using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// Модель для добавления/удаления администратора подразделения
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class AddAdminDepartmentAdmin
    {
        /// <summary>
        /// Отдел
        /// </summary>
        [Required]
        public int DepartmentId { get; set; }

        /// <summary>
        /// Сотрудник
        /// </summary>
        [Required]
        public int EmployeeId { get; set; }
    }
}
