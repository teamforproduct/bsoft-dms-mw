namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminLanguageValue
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID языка
        /// </summary>
        public int LanguageId { get; set; }
        /// <summary>
        /// Метка
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
    }
}