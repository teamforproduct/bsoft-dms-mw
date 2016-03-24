using BL.Model.Users;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для перезаписи списка тегов документа
    /// </summary>
    public class ModifyDocumentTasks : CurrentPosition
    {
        /// <summary>
        /// ИД Task
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// ИД Позиции
        /// </summary>
        public int PositionId { get; set; }
        /// <summary>
        /// ИД исполняющего агента
        /// </summary>
        public int PositionExecutorAgentId { get; set; }
        /// <summary>
        /// ИД Агента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
    }
}
