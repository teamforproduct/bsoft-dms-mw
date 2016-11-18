using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BL.Model.DocumentCore.Filters
{
    /// <summary>
    /// Фильтр событий документов
    /// </summary>
    public class FilterDocumentEventNatively
    {

        /// <summary>
        /// Массив ИД должностей получателей событий по документу
        /// </summary>
        public List<int> SourcePositionIDs { get; set; }
        public Period Date { get; set; }

        public List<int> SourcePositionExecutorAgentIDs { get; set; }
        public List<int> SourceAgentIDs { get; set; }
        public List<int> TargetPositionIDs { get; set; }
        public List<int> TargetPositionExecutorAgentIDs { get; set; }

        public Period ReadDate { get; set; }
        public List<int> ReadAgentIDs { get; set; }
    }
}
