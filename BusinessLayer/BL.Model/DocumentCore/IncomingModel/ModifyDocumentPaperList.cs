using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель модификации реестров БН
    /// </summary>
    public class ModifyDocumentPaperList
    {
        /// <summary>
        /// ИД БН
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }

    }
}
