﻿using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.AdminCore
{
    /// <summary>
    /// Модель для проверки прав доступа
    /// </summary>
    public class VerifyAccess
    {
        public VerifyAccess()
        {
        }
        public VerifyAccess(int userId)
        {
            UserId = userId;
        }
        /// <summary>
        /// ИД юзера, по умолчанию возьмет из контекста
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Код действия
        /// </summary>
        public EnumActions? ActionCode { get; set; }
        /// <summary>
        /// ИД записи, если права должны раздавать в разрезе каждой записи объекта
        /// </summary>
        public int? RecordId { get; set; }
        /// <summary>
        /// ИД должности, для которой нужно проверить права
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// Массив ИД должностей, от которых работает пользователь
        /// </summary>
        public List<int> PositionsIdList { get; set; }
    }
}