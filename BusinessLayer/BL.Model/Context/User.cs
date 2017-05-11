namespace BL.Model.Context
{
    /// <summary>
    /// класс сотрудника
    /// </summary>
    public class User
    {
        /// <summary>
        /// ИД веб пользователя
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name веб пользователя
        /// </summary>
        public string Name { get; set; }

        public string Fingerprint { get; set; }

        /// <summary>
        /// Код языка
        /// </summary>
        public int LanguageId { get; set; }

        public bool IsChangePasswordRequired { get; set; }

    }
}