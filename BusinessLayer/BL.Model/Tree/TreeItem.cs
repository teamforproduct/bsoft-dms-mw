using BL.Model.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.Tree
{
    /// <summary>
    /// Элемент дерева (Node)
    /// </summary>
    public class TreeItem : DictionaryItem, ITreeItem
    {
        /// <summary>
        /// Текст для поиска
        /// </summary>
        [IgnoreDataMember]
        public string SearchText { get; set; }

        /// <summary>
        /// Уникальный ключ элемента
        /// </summary>
        public string TreeId { get; set; }

        /// <summary>
        /// Уникальный ключ родительского элемента
        /// </summary>
        //[IgnoreDataMember]
        public string TreeParentId { get; set; }

        /// <summary>
        /// Тип объекта для деревьев с разнородными сущностями. EnumSystemObject
        /// </summary>
        public int? ObjectId { get; set; }

        /// <summary>
        /// Уровень вложенности элемента
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// Вспомагательный признак для ускорения построения дерева.
        /// </summary>
        [IgnoreDataMember]
        public bool IsUsed { get; set; }

        /// <summary>
        /// Полный путь, который формирует универсальная процедура построения дерева.
        /// </summary>
        [IgnoreDataMember]
        public string Path { get; set; }

        /// <summary>
        /// Лист или группа. У листьев нет наследников
        /// </summary>
        public bool? IsLeaf { get; set; }

        /// <summary>
        /// Отмечен.
        /// </summary>
        public bool? IsChecked { get; set; }

        

        /// <summary>
        /// Список потомков, который формирует универсальная процедура построения дерева.
        /// </summary>
        public IEnumerable<TreeItem> Childs { get; set; }
    }
}
