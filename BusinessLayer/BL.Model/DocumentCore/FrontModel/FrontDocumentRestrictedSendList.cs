using System;
using BL.Model.DocumentCore.IncomingModel;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Фронтовая модель ограничительного списка
    /// </summary>
    public class FrontDocumentRestrictedSendList
    {
        /// <summary>
        /// ИД записи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД должности
        /// </summary>
        public int? PositionId { get; set; }
        /// <summary>
        /// ИД уровня доступа
        /// </summary>
        public EnumAccessLevels? AccessLevel { get; set; }
        public string PositionName { get; set; }
        public string PositionExecutorAgentName { get; set; }
        public string AccessLevelName { get; set; }
        public string DepartmentName { get; set; }
    }
}
