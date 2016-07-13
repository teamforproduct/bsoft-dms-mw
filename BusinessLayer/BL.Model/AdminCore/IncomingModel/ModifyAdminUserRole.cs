using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// "Соответствие ролей и пользователя", добавления/редактирования записи.
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyAdminUserRole
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Дата назначения роли
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата снятия роли
        /// </summary>
        public DateTime EndDate { get; set; }

    }
}