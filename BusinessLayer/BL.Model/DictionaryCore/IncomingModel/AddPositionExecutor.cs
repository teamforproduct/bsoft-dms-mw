using BL.Model.Enums;
using BL.Model.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// При назначении сотрудника на должность нужно указать следующие парметры:
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class AddPositionExecutor
    {
        
        /// <summary>
        /// Признак активности.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Сотрудник-пользователь-агент (Id совпадают)
        /// </summary>
        [Required]
        public int AgentId { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        [Required]
        public int PositionId { get; set; }

        /// <summary>
        /// Тип исполнения: 
        /// </summary>
        [Required]
        public EnumPositionExecutionTypes PositionExecutorTypeId { get; set; }

        /// <summary>
        /// Уровень доступа к документам: лично, референт, ио
        /// При создании документов всегда указывается уровень доступа для ио и референтов
        /// </summary>
        [Required]
        public EnumAccessLevels AccessLevelId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата начала исполнения должности
        /// </summary>
        [Required]
        public DateTime StartDate { get { return _StartDate; } set { _StartDate=value.ToUTC(); } }
        private DateTime _StartDate;

        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        //[Required]
        public DateTime? EndDate { get { return _EndDate; } set { _EndDate=value.ToUTC(); } }
        private DateTime? _EndDate;

    }
}
