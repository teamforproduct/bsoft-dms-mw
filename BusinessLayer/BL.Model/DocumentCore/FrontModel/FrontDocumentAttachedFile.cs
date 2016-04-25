using System;
using BL.Model.DocumentCore.InternalModel;

namespace BL.Model.DocumentCore.FrontModel
{
    /// <summary>
    /// Модель файла документа для отображения пользователю
    /// </summary>
    public class FrontDocumentAttachedFile : FrontRegistrationFullNumber
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FrontDocumentAttachedFile()
        {
        }

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
        public bool IsAdditional { get; set; }
        /// <summary>
        /// содержимое файла
        /// </summary>
        public byte[] FileContent { get; set; }
        /// <summary>
        /// Название файла без расширения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Расширение файла
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// Тип файла
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Размер файла
        /// </summary>
        public int FileSize { get; set; }

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
        public DateTime Date { get; set; }
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
        public DateTime LastChangeDate { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionExecutorAgentName { get; set; }
    }
}