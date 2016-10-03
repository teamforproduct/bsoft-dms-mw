using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Internal элемент справочника "Исполнители"
    /// </summary>
    public class InternalDictionaryPositionExecutor : LastChangeInfo
    {

        public InternalDictionaryPositionExecutor()
        { }

        public InternalDictionaryPositionExecutor(ModifyDictionaryPositionExecutor model)
        {
            Id = model.Id;
            IsActive = model.IsActive;
            AgentId = model.AgentId;
            PositionId = model.PositionId;
            PositionExecutorTypeId = (int)model.PositionExecutorTypeId;
            AccessLevelId = (int)model.AccessLevelId;
            Description = model.Description;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Агент
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Тип исполнителя
        /// </summary>
        public int PositionExecutorTypeId { get; set; }

        /// <summary>
        /// Уровень доступа: лично, референт, ио
        /// </summary>
        public int AccessLevelId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата начала исполнения должности
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания исполнения должности
        /// </summary>
        public DateTime EndDate { get; set; }


    }
}