using System;
using BL.Model.DocumentCore.FrontModel;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.InternalModel
{
    /// <summary>
    /// Класс приатаченного к документу файла для внутреннего использования
    /// </summary>
    public class InternalDocumentAttachedFile : InternalTemplateAttachedFile
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public InternalDocumentAttachedFile()
        {
        }

        /// <summary>
        /// Версия вложения
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Дата создания файла
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Признак, изменялся ли файл в хранищие извне
        /// </summary>
        public bool WasChangedExternal { get; set; }

        public int ExecutorPositionId { get; set; }
        public int ExecutorPositionExecutorAgentId { get; set; }
        public IEnumerable<InternalDocumentEvent> Events { get; set; }
    }
}