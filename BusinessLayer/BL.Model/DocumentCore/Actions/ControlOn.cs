using BL.Model.Users;
using System;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для взятия документа на контроль
    /// </summary>
    public class ControlOn : CurrentPosition
    {
        public ControlOn()
        {
        }

        public ControlOn(ControlChange controlChange, int documentId, string task)
        {
            DocumentId = documentId;
            Task = task;
            Description = controlChange.Description;
            DueDate = controlChange.DueDate;
            AttentionDate = controlChange.AttentionDate;
        }

        public ControlOn(InternalDocumentSendList sendList)
        {
            DocumentId = sendList.DocumentId;
            Task = sendList.Task;
            Description = sendList.Description;
            DueDate = new[] {sendList.DueDate ?? DateTime.Now, DateTime.Now.AddDays(sendList.DueDay ?? 0)}.Max();
        }

        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Контрольный срок
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Срок включения режима постоянное внимание
        /// </summary>
        public DateTime? AttentionDate { get; set; }
    }
}
