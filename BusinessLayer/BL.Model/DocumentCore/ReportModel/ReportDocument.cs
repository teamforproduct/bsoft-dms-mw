using System.Collections.Generic;

namespace BL.Model.DocumentCore.ReportModel
{
    public class ReportDocument 
    {
        public ReportDocument()
        {
        }
        /// <summary>
        /// ИД Документа
        /// </summary>
        public int Id { get; set; }

        public string DocumentTypeName { get; set; }

        public string ExecutorPositionName { get; set; }
        /// <summary>
        /// Кому адресован документ
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// Краткое содержание
        /// </summary>
        public string Description { get; set; }
       
        public IEnumerable<ReportDocumentWait> DocumentWaits { get; set; }

    }
}