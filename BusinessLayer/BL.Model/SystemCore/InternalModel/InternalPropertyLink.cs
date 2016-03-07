using BL.Model.Common;

namespace BL.Model.SystemCore.InternalModel
{
    public class InternalPropertyLink : LastChangeInfo
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int ObjectId { get; set; }
        public string Filers { get; set; }
        public bool IsMandatory { get; set; }
        
        public InternalProperty Property { get; set; }
        
        public InternalSystemObject Object { get; set; }
    }
}