using BL.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Tree
{
    /// <summary>
    /// Контракт для построения деревьев
    /// </summary>
    public interface ITreeItem: IDictionaryItem
    {
        /// <summary>
        /// Уникальный ключ элемента
        /// </summary>
        string TreeId { get; set; }

        /// <summary>
        /// Уникальный ключ родительского элемента
        /// </summary>
        string TreeParentId { get;  }

        /// <summary>
        /// Тип объекта для деревьев с разнородными сущностями. EnumSystemObject
        /// </summary>
        int? ObjectId { get;  }

        /// <summary>
        /// Вспомагательный признак для ускорения построения дерева.
        /// </summary>
        bool IsUsed { get; set; }

        /// <summary>
        /// Полный путь, который формирует универсальная процедура построения дерева.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Лист или группа. У листьев нет наследников
        /// </summary>
        bool? IsList { get; set; }

        /// <summary>
        /// Уровень вложенности элемента
        /// </summary>
        int? Level { get; set; }

        /// <summary>
        /// Список потомков, который формирует универсальная процедура построения дерева.
        /// </summary>
        IEnumerable<TreeItem> Childs { get; set; }
    }
}
