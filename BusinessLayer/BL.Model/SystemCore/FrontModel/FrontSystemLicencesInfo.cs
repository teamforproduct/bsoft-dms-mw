using BL.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Model.SystemCore.FrontModel
{
    /// <summary>
    /// описание результатов проверки лицензии
    /// </summary>
    public class FrontSystemLicencesInfo
    {
        /// <summary>
        /// код важности сообщения
        /// </summary>
        public EnumMessageLevelTypes MessageLevelTypes { get; set; }
        /// <summary>
        /// название уровня важности сообщения
        /// </summary>
        public string MessageLevelTypesName { get; set; }
        /// <summary>
        /// сообщение
        /// </summary>
        public string Message { get; set; }
    }
}
