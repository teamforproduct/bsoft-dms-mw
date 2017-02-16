using BL.Model.AdminCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Настройка правил рассылки между должностями (для исполнения, для сведения)", представление записи.
    /// </summary>
    public class FrontAdminSubordination : SetSubordination
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }


        /// <summary>
        /// Руководитель
        /// </summary>
        public string SourcePositionName { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public string TargetPositionName { get; set; }

        /// <summary>
        /// Тип рассылки (для исполнения, для сведения)
        /// </summary>
        public string SubordinationTypeName { get; set; }

    }
}