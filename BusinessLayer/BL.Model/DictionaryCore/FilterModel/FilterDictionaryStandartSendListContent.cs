using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryStandartSendListContent : DictionaryBaseFilterParms
    /// <summary>
    /// фильтр содержания типовой рассылки
    /// </summary>
    public class FilterDictionaryStandartSendListContent
    {

        /// <summary>
        /// список типовых рассылок
        /// </summary>
        public List<int> StandartSendListId { get; set; }
        /// <summary>
        /// игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }
        /// <summary>
        /// список типов рассылок
        /// </summary>
        public List<int> SendTypeId { get; set; }
        /// <summary>
        /// Должность получателя
        /// </summary>
        public int? TargetPositionId { get; set; }
        /// <summary>
        /// ПОльзователь-получатель
        /// </summary>
        public int? TargetAgentId { get; set; }
        /// <summary>
        /// Задача
        /// </summary>
        public string Task { get; set; }
        /// <summary>
        /// Название типа рассылки
        /// </summary>
        public string SendTypeName { get; set; }
        /// <summary>
        /// Название должности получателя
        /// </summary>
        public string TargetPositionName { get; set; }
        /// <summary>
        /// ФИО получателя
        /// </summary>
        public string TargetAgentName { get; set; }

    }
}
