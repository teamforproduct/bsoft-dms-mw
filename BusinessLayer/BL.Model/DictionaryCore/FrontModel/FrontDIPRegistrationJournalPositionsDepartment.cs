using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Tree;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    public class FrontDIPRegistrationJournalPositionsDepartment : FrontDIPRegistrationJournalPositionsBase
    {

        /// <summary>
        /// Код отдела
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public int? DepartmentId { get; set; }
        
    }
}