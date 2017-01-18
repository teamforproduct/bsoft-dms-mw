using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;
using System;
using BL.Model.Enums;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalPermissions
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Тип доступа
        /// </summary>
        public EnumAccessTypes AccessTypeId { get; set; }
        public string AccessTypeCode { get; set; }
        public string AccessTypeName { get; set; }
        public int AccessTypeOrder { get; set; }


        /// <summary>
        /// Модуль
        /// </summary>
        public int ModuleId { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleName { get; set; }
        public int ModuleOrder { get; set; }

        /// <summary>
        /// Фича
        /// </summary>
        public int FeatureId { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public int FeatureOrder { get; set; }

        public bool IsChecked { get; set; }

    }
}