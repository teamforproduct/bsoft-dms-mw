using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.FrontModel
{
    public class FrontFeature
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Чтение
        /// </summary>
        public FrontPermissionValue Read { get; set; }

        /// <summary>
        /// Чтение
        /// </summary>
        public FrontPermissionValue Create { get; set; }

        /// <summary>
        /// Чтение
        /// </summary>
        public FrontPermissionValue Update { get; set; }

        /// <summary>
        /// Удаление
        /// </summary>
        public FrontPermissionValue Delete { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Order { get; set; }

    }
}