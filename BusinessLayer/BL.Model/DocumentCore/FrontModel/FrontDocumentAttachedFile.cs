using System;
using BL.Model.Enums;
using BL.Model.Extensions;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель файла документа для отображения пользователю
    /// </summary>
    public class FrontDocumentAttachedFile : FrontRegistrationFullNumber
    {
        /// <summary>
        /// ИД.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер вложения в документе
        /// </summary>
        public int OrderInDocument { get; set; }
        /// <summary>
        /// Признак дополнительный файл или основной
        /// </summary>
        public EnumFileTypes Type { get; set; }
        /// <summary>
        /// Признак дополнительный файл или основной
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// Признак основная версия файла
        /// </summary>
        public bool IsMainVersion { get; set; }

        /// <summary>
        /// Признак удаленной версии файла
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Признак обработаной версии файла
        /// </summary>
        public bool IsWorkedOut { get; set; }
        /// <summary>
        /// Есть ли необработанные версии файлов
        /// </summary>
        public bool IsNotAllWorkedOut { get; set; }
        /// <summary>
        /// содержимое файла
        /// </summary>
        public string FileContent { get; set; }
        /// <summary>
        /// Название файла без расширения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Описание файла
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Тип файла
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Размер файла
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Хэш файла. для проверки целостности файла в хранилище
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Версия вложения
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Дата создания файла
        /// </summary>
        public DateTime Date { get { return _Date; } set { _Date=value.ToUTC(); } }
        private DateTime  _Date; 
		
        /// <summary>
        /// Признак, изменялся ли файл в хранищие извне
        /// </summary>
        public bool WasChangedExternal { get; set; }

        /// <summary>
        /// Имя пользователя, который последний редактировал файл
        /// </summary>
        public string LastChangeUserName { get; set; }

        /// <summary>
        /// ИД пользователя, последним изменившего запись
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата последнего изменения записи
        /// </summary>
        public DateTime LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate=value.ToUTC(); } }
        private DateTime  _LastChangeDate; 
		
        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionExecutorAgentName { get; set; }
        public bool PdfCreated { get; set; }
        public DateTime? LastPdfAccess { get; set; }
    }
}