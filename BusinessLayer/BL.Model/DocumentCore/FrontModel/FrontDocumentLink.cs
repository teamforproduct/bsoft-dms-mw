
using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentLink: FrontRegistrationFullNumber
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// общая информация о связи
        /// </summary>
        public string LinkTypeName { get; set; }

        /// <summary>
        /// Информация о радительском документе 
        /// </summary>
        public bool IsParent { get; set; }
        /// <summary>
        /// ИД должности, от которой сделана связь
        /// </summary>
        public int? ExecutorPositionId { get; set; }
        /// <summary>
        /// Название должности, от которой сделана связь
        /// </summary>
        public string ExecutorPositionName { get; set; }
        /// <summary>
        /// ИД исполнителя по должности, на момент создания связи
        /// </summary>
        public int? ExecutorPositionExecutorAgentId { get; set; }
        /// <summary>
        ///  Название исполнителя по должности, на момент создания связи
        /// </summary>
        public string ExecutorPositionExecutorAgentName { get; set; }
        /// <summary>
        /// Может ли пользователь удалить связь
        /// </summary>
        public bool? CanDelete { get; set; }

    }
}
