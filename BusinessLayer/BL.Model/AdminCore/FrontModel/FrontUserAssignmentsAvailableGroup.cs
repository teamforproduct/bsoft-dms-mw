using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие ролей и пользователя", описывает доступные пользователю роли, представление записи.
    /// </summary>
    public class FrontUserAssignmentsAvailableGroup
    {

        /// <summary>
        /// ИД Тип исполнения
        /// </summary>
        public int ExecutorTypeId { get; set; }

        /// <summary>
        /// ИД Тип исполнения
        /// </summary>
        public string ExecutorTypeDescription { get; set; }

        /// <summary>
        /// Список назначений
        /// </summary>
        public List<FrontUserAssignmentsAvailable> Assignments { get; set; }

    }
}