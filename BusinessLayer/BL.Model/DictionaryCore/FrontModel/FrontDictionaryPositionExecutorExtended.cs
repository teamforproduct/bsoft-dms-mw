using BL.Model.AdminCore.FrontModel;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Исполнители". 
    /// </summary>
    public class FrontDictionaryPositionExecutorExtended 
    {

        /// <summary>
        /// ID
        /// </summary>
        public int AssignmentId { get; set; }

        /// <summary>
        /// Признак активности.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Дата начала исполнения должности
        /// </summary>
        public DateTime? StartDate { get { return _StartDate; } set { _StartDate=value.ToUTC(); } }
        private DateTime?  _StartDate; 


        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        //[Required]
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate=value.ToUTC(); } }
        private DateTime?  _EndDate; 


        /// <summary>
        /// Агент
        /// </summary>
        public string ExecutorName { get; set; }

        /// <summary>
        /// Тип исполнения
        /// </summary>
        public string PositionExecutorSuffix { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionName { get; set; }


        /// <summary>
        /// Департамент
        /// </summary>
        public string DepartmentCodeName { get; set; }

        /// <summary>
        ///  Роли должности
        /// </summary>
        public IEnumerable<FrontAdminPositionRole> PositionRoles { get; set; }

    }
}