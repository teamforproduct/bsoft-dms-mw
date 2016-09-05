using BL.Model.DictionaryCore.FrontModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// При назначении сотрудника на должность нужно указать следующие парметры:
    /// </summary>
    // В модели перечислены поля, на значения которых можно повлиять из интерфейса. Например поля таблицы LastChangeUserId и LastChangeDate в этой модели отсутствуют
    // Если в таблице поля объявлены как Nullable то поля в этом классе нужно объявлять Nullable
    public class ModifyDictionaryPositionExecutor
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        
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
        /// 1 - Назначен на должность;
        /// 2 - Исполяет обязанности;
        /// 3 - Является референтом;
        /// </summary>
        [Required]
        public int PositionExecutorTypeId { get; set; }

        /// <summary>
        /// Уровень доступа к документам: лично, референт, ио
        /// При создании документов всегда указывается уровень доступа для ио и референтов
        /// </summary>
        [Required]
        public int AccessLevelId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата начала исполнения должности
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

    }
}
