using BL.Model.Tree;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    // 
    public class FrontDIPSubordinationsBase: TreeItem
    {
  
        /// <summary>
        /// Для сведения
        /// </summary>
        public int? IsInforming { get; set; }

        /// <summary>
        /// Для исполнения
        /// </summary>
        public int? IsExecution { get; set; }

    }
}