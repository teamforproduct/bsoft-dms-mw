using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// "Соответствие действий и роли", добавления/редактирования записи.
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class SetRolePermissionByModuleFeature
    {
        /// <summary>
        /// Роль
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// Модуль
        /// </summary>
        [Required]
        public string Module { get; set; }

        /// <summary>
        /// Фича
        /// </summary>
        [Required]
        public string Feature { get; set; }

        /// <summary>
        /// Галочка
        /// </summary>
        [Required]
        public bool IsChecked { get; set; }

    }
}