namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminLanguage
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код языка
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Название языка
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Язык по умолчанию
        /// </summary>
        public bool IsDefault { get; set; }
    }
}