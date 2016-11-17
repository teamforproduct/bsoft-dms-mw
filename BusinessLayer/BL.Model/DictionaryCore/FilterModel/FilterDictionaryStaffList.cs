using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryDepartment
    /// </summary>
    // В этой модели целесообразно все поля, объявленные простыми типами, делать Nullable, чтобы при формировании Where можно было проверить на if != null
    public class FilterDictionaryStaffList : Tree.FilterTree
    {

        /// <summary>
        /// Сужение по отделам. Отсекает дочерние отделы
        /// </summary>
        public List<int> DepartmentIDs { get; set; }
        
    }
}
