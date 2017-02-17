using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Сведения об авторизации сотрудника - пользователя
    /// </summary>
    public class FrontUserFavorites
    {
        public List<int> ListEmployees { get; set; }

        public List<int> ListCompanies { get; set; }

        public List<int> ListJournals { get; set; }

        public List<int> ListPersons { get; set; }

        public List<int> ListPositions { get; set; }

        public List<int> ListTags { get; set; }

        public List<int> ListDepartments { get; set; }
    }
}