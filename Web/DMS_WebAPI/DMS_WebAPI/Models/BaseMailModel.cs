namespace DMS_WebAPI.Models
{
    public class BaseMailModel
    {
        /// <summary>
        /// Приветствие
        /// </summary>
        public string Greeting { get; set; }

        /// <summary>
        /// Целевая кнопка. Название
        /// </summary>
        public string CallToActionName { get; set; }

        /// <summary>
        /// Целевая кнопка. Ссылка
        /// </summary>
        public string CallToActionUrl { get; set; }

        /// <summary>
        /// Завершение
        /// </summary>
        public string Closing { get; set; }
    }
}