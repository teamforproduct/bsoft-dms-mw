using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.IncomingModel
{
    public class AddTemplateDocumentPaper
    {
        /// <summary>
        /// ИД документа
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Название бумажного насителя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Комментарий по бумажному носителю
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак является ли бумажный носитель основным документом (или приложением)
        /// </summary>
        public bool IsMain { get; set; }
        /// <summary>
        /// Признак является ли бумажный носитель оригиналом (или заверенной копией)
        /// </summary>
        public bool IsOriginal { get; set; }
        /// <summary>
        /// Признак является ли бумажный носитель копией
        /// </summary>
        public bool IsCopy { get; set; }
        /// <summary>
        /// Количество страниц в бумажном носителе
        /// </summary>
        public int PageQuantity { get; set; }
        /// <summary>
        /// Количество бумажных носителей для добавления
        /// </summary>
        public int PaperQuantity { get; set; }
    }
}
