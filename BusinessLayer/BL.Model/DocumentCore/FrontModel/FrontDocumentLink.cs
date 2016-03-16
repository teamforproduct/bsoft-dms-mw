
using System;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentLink
    {
        /// <summary>
        /// ID 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// общая информация о связи
        /// </summary>
        public string LinkTypeName { get; set; }
        public string RegistrationFullNumber { get; set; }
        public DateTime? DocumentDate { get; set; }

    }
}
