using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    public class FrontPermissionValue
    {
        /// <summary>
        /// Модуль
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Фича
        /// </summary>
        public string Feature { get; set; }

        /// <summary>
        /// Тип доступа
        /// </summary>
        public string AccessType { get; set; }


        public EnumAccessTypesValue Value { get; set; }


    }
}