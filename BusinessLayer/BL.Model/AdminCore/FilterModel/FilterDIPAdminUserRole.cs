﻿using BL.Model.Extensions;
using BL.Model.Tree;
using System;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterAdminUserRole
    /// </summary>
    public class FilterDIPAdminUserRole : FilterTree
    {
        /// <summary>
        /// Список ID
        /// </summary>
        public List<int> IDs { get; set; }

        /// <summary>
        /// Исключение записей по ID
        /// </summary>
        public List<int> NotContainsIDs { get; set; }


        /// <summary>
        /// Должности
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Назначение
        /// </summary>
        public int? PositionExecutorId { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate = value.ToUTC(); } }
        private DateTime? _StartDate;

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate = value.ToUTC(); } }
        private DateTime? _EndDate;

    }
}