
using System;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    public class FrontDocumentLinkShot
    {
        /// <summary>
        /// Список ИД документов, всходящих в пакет 
        /// </summary>
        public IEnumerable<int> DocumentId { get; set; }
        /// <summary>
        /// ИД пакета связанных документов 
        /// </summary>
        public int? LinkId { get; set; }
    }
}
