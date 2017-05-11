using BL.Model.Users;
using System;
using System.Linq;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для взятия документа на контроль
    /// </summary>
    public class ControlOn : AddNote
    {
        public ControlOn()
        {
        }

        public ControlOn(ControlChange controlChange, int documentId, string task = null)
        {
            DocumentId = documentId;
            Task = task;
            Description = controlChange.Description;
            DueDate = controlChange.DueDate;
            AttentionDate = controlChange.AttentionDate;
            TargetCopyAccessGroups = controlChange.TargetCopyAccessGroups;
            EventDate = controlChange.EventDate;
        }
        /*
        public ControlOn(InternalDocumentSendList sendList)
        {
            DocumentId = sendList.DocumentId;
            TaskName = sendList.TaskName;
            Description = sendList.Description;
            DueDate = new[] {sendList.DueDate ?? DateTime.UtcNow, DateTime.UtcNow.AddDays(sendList.DueDay ?? 0)}.Max();
        }
        */
        /// <summary>
        /// Контрольный срок
        /// </summary>
        public DateTime? DueDate { get { return _DueDate; } set { _DueDate = value.ToUTC(); } }
        private DateTime? _DueDate;
        /// <summary>
        /// Срок включения режима постоянное внимание
        /// </summary>
        public DateTime? AttentionDate { get { return _AttentionDate; } set { _AttentionDate = value.ToUTC(); } }
        private DateTime? _AttentionDate;
    }
}
