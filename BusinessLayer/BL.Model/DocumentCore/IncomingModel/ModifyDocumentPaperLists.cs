using BL.Model.Users;
using System;
using System.Runtime.Serialization;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Модель для перезаписи списка тегов документа
    /// </summary>
    public class ModifyDocumentPaperLists : CurrentPosition
    {
        /// <summary>
        /// ИД Task
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
