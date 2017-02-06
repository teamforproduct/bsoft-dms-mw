using BL.Model.Tree;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryJournalsTree : FilterTree
    {
        /// <summary>
        /// Если flse склывает отделы без журналов
        /// </summary>
        public bool? IsShowAll {get; set;}
    }
}
