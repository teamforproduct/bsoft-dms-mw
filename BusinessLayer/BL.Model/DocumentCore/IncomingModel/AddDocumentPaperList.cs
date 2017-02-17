using BL.Model.Users;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для формирования реестров БН
    /// </summary>
    public class AddDocumentPaperList
    {
        /// <summary>
        /// Комментарий к реестру
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Массив ИД должностей от кого идет рассылка (если не указано - от всех позиций, от которых работает пользователь)
        /// </summary>
        public List<int> SourcePositionIds { get; set; }
        /// <summary>
        /// Массив ИД должностей на кого идет рассылка (если не указано - на все позиции)
        /// </summary>
        public List<int> TargetPositionIds { get; set; }
        /// <summary>
        /// Массив ИД должностей на кого идет рассылка (если не указано - на все позиции)
        /// </summary>
        public List<int> TargetAgentIds { get; set; }

    }
}
