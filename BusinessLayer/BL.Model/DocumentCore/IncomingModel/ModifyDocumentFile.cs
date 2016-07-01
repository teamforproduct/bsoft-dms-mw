using BL.Model.Users;
using System.Runtime.Serialization;
using System.Web;

namespace BL.Model.DocumentCore.IncomingModel
{
    /// <summary>
    /// Добавляемый или редактируемый файл документа
    /// </summary>
    public class ModifyDocumentFile : CurrentPosition
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Ид. документа, которому принадлежит файл
        /// </summary>
        public int DocumentId { get; set; }
        /// <summary>
        /// Порядковый номер файла в списке файлов документа
        /// Только для изменения файла и для добавления версию файла к файлу
        /// </summary>
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Версия файла
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Название файла
        /// Только для изменения имени файла
        /// </summary>
        public string FileName { get; set; }
    }
}
