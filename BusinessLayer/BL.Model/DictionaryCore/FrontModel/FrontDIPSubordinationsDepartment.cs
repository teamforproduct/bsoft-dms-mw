﻿using BL.Model.Common;
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
    public class FrontDIPSubordinationsDepartment: FrontDIPSubordinationsBase
    {

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Руководитель
        /// </summary>
        public int? SourcePositionId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? DepartmentId { get; set; }
        
    }
}