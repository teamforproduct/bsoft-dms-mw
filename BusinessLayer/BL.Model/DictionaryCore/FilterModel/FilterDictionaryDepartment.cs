using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryDepartment
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryDepartment : DictionaryBaseFilterParms
    {

        /// <summary>
        /// Сужение по полному наименованию подразделений
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Индекс подразделения
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Сужение по компании, которая представляет этот отдел
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        public int? ChiefPositionId { get; set; }
        
    }
}
