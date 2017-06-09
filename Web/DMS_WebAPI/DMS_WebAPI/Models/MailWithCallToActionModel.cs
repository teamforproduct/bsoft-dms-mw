namespace DMS_WebAPI.Models
{
    public class MailWithCallToActionModel
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
        /// Целевая кнопка. Описание, почему я должен нажать на целевую кнопку
        /// </summary>
        public string CallToActionDescription { get; set; }

        /// <summary>
        /// Постскриптум, приписка
        /// </summary>
        public string PostScriptum { get; set; }

        /// <summary>
        /// Завершение
        /// </summary>
        public string Closing { get; set; }
    }
}