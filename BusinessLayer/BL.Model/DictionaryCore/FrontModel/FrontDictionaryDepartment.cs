using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Структура предприятия"
    /// </summary>
    // 
    public class FrontDictionaryDepartment: ModifyDictionaryDepartment, ITreeItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Наименование вышестоящего подразделения
        /// </summary>
        public string ParentDepartmentName { get; set; }
        
        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        public string ChiefPositionName { get; set; }

        /// <summary>
        /// Компания
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Список подчиненных подразделений
        /// </summary>
        public virtual IEnumerable<FrontDictionaryDepartment> ChildDepartments { get; set; }

        #region ITreeItem

        public int ObjectId
        {
            get { return (int)EnumObjects.DictionaryDepartments; }
        }

        public ITreeItem Parent
        {
            get { return null; }
        }

        public IEnumerable<ITreeItem> Childs { get; set; }
        //{
        //    get { return ChildPositions; }
        //    set { ChildPositions = (IEnumerable<FrontDictionaryPosition>)value; }
        //}


        #endregion

    }
}