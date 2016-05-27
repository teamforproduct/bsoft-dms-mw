using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Исполнители". 
    /// </summary>
    public class FrontDictionaryPositionExecutor : ModifyDictionaryPositionExecutor
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Агент
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionFullName { get; set; }

        /// <summary>
        /// Тип исполнителя
        /// </summary>
        public string PositionExecutorTypeName { get; set; }

        /// <summary>
        /// Уровень доступа: лично, референт, ио
        /// </summary>
        public string AccessLevelName { get; set; }




    }
}