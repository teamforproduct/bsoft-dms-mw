﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.IncomingModel
{
    /// <summary>
    /// "Настройка правил рассылки между должностями (для исполнения, для сведения)", добавления/редактирования записи.
    /// </summary>
    // В модели перечислены поля, значения которых можно изменить из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyAdminSubordination
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        public int SourcePositionId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int TargetPositionId { get; set; }

        /// <summary>
        /// Тип рассылки (для исполнения, для сведения)
        /// </summary>
        public int SubordinationTypeId { get; set; }

    }
}