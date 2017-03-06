using BL.Model.Extensions;
using System;
using System.Runtime.Serialization;

namespace BL.Model.AdminCore.FrontModel
{
    /// <summary>
    /// "Соответствие ролей и пользователя", описывает доступные пользователю роли, представление записи.
    /// </summary>
    public class FrontUserAssignmentsAvailable
    {
        /// <summary>
        /// Признак, выбиралась ли позиция при последнем входе
        /// </summary>
        public bool IsLastChosen { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Должность. Наименование
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public string ExecutorName { get; set; }

        /// <summary>
        /// Исполнитель. Аватарка
        /// </summary>
        public string ExecutorImage { get { return Converter.ToBase64String(imageByteArray); } }

        [IgnoreDataMember]
        public byte[] ImageByteArray { set { imageByteArray = value; } }
        private byte[] imageByteArray;


        /// <summary>
        /// ИД Тип исполнения
        /// </summary>
        [IgnoreDataMember]
        public int ExecutorTypeId { get; set; }

        /// <summary>
        /// ИД Тип исполнения
        /// </summary>
        [IgnoreDataMember]
        public string ExecutorTypeDescription { get; set; }



        /// <summary>
        /// Количество непрочитанных событий
        /// </summary>
        public int NewEventsCount { get; set; }

        /// <summary>
        /// Количество контролей
        /// </summary>
        public int ControlsCount { get; set; }

        /// <summary>
        /// Количество просроченных контролей
        /// </summary>
        public int OverdueControlsCount { get; set; }

        /// <summary>
        /// Минимальная контрольная дата  
        /// </summary>
        public DateTime? MinDueDate { get { return _MinDueDate; } set { _MinDueDate = value.ToUTC(); } }
        private DateTime? _MinDueDate;

    }
}