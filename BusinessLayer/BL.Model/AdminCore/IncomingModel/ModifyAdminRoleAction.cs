﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using BL.Model.Enums;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// "Соответствие действий и роли", добавления/редактирования записи.
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyAdminRoleAction
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// RecordId
        /// </summary>
        public int? RecordId { get; set; }

    }
}