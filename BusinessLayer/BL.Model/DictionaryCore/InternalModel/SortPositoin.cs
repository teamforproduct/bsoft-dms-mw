using System;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class SortPositoin : IComparable
    {
        public int Id { get; set; }

        public int OldOrder { get; set; }

        public int NewOrder { get; set; }

        public int CompareTo(object obj)
        {
            return NewOrder - (obj as SortPositoin).NewOrder;
        }
    }
}
