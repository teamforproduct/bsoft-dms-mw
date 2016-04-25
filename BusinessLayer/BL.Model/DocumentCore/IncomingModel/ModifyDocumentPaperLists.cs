using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель модификации реестров БН
    /// </summary>
    public class ModifyDocumentPaperLists
    {
        /// <summary>
        /// ИД БН
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Description { get; set; }

    }
}
