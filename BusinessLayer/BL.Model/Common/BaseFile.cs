namespace BL.Model.Common
{
    public class BaseFile
    {
        /// <summary>
        /// содержимое файла
        /// </summary>
        public byte[] FileContent { get; set; }
        /// <summary>
        /// Название файла
        /// </summary>
        public string FileName { get { return Name + "." + Extension; } }
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
        public long? FileSize { get; set; }
    }
}
