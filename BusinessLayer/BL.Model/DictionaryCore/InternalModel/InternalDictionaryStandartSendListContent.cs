using BL.Model.Common;
using BL.Model.Enums;
using System;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryStandartSendListContent: LastChangeInfo
    {

        public InternalDictionaryStandartSendListContent() { }

        public InternalDictionaryStandartSendListContent(ModifyDictionaryStandartSendListContent model)
        {
            Id = model.Id;
            StandartSendListId = model.StandartSendListId;
            Stage = model.Stage;
            SendType = model.SendTypeId;
            TargetPositionId = model.TargetPositionId;
            TargetAgentId = model.TargetAgentId;
            Task = model.Task;
            Description = model.Description;
            DueDate = model.DueDate;
            DueDay = model.DueDay;
            AccessLevel = model.AccessLevelId;

        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ссылка на типовой список рассылки
        /// </summary>
        public int StandartSendListId { get; set; }
        /// <summary>
        /// Этап
        /// </summary>
        public int Stage { get; set; }
        /// <summary>
        /// Тип рассылки
        /// </summary>
        public EnumSendTypes SendType { get; set; }
        /// <summary>
        /// Должность получателя
        /// </summary>
        public int? TargetPositionId { get; set; }
        /// <summary>
        /// Агент-получатель
        /// </summary>
        public int? TargetAgentId { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Комментарии
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Выполнить до
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// Количество дней
        /// </summary>
        public int? DueDay { get; set; }
        /// <summary>
        /// Уровень доступа
        /// </summary>
        public EnumDocumentAccesses? AccessLevel { get; set; }
    }
}
