
using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentLink: FrontRegistrationFullNumber
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// общая информация о связи
        /// </summary>
        public string LinkTypeName { get; set; }

        public DateTime? DocumentDate { get; set; }

    }
}
