namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalActionRecord
    {
        public int? Id { get; set; }
        public int? DocumentId { get; set; }
        public int? WaitId { get; set; }
        public int? EventId { get; set; }
        public int? SubscriptionId { get; set; }
        public int? SendListId { get; set; }
        public int? PaperId { get; set; }

        public string Description { get; set; }

    }
}
