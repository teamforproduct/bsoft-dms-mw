using System;
using BL.Model.Users;
using System.ComponentModel.DataAnnotations;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель для прошения о переносе сроков исполнения
    /// </summary>
    public class AskPostponeDueDate: SendEventMessage
    {
        /// <summary>
        /// Предложение по новому сроку
        /// </summary>
        public DateTime? PlanDueDate { get { return _planDueDate; } set { _planDueDate = value.ToUTC(); } }
        private DateTime? _planDueDate;
    }
}
