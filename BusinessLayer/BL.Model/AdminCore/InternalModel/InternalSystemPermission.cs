namespace BL.Model.AdminCore.InternalModel
{
    public class InternalSystemPermission
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int FeatureId { get; set; }
        public int AccessTypeId { get; set; }
        public int ModuleOrder { get; set; }
        public string ModuleCode { get; set; }
        public int FeatureOrder { get; set; }
        public string FeatureCode { get; set; }
        public int AccessTypeOrder { get; set; }
        public string AccessTypeCode { get; set; }
    }
}