namespace BL.Model.DocumentCore
{
    public class FullDocument : InternalDocument
    {
        public FullDocument()
        {
        }

        public FullDocument(ModifyDocument model, FullDocument doc) : this()
        {
            if (model != null)
            {
                Id = model.Id;
                DocumentSubjectId = model.DocumentSubjectId;
                Description = model.Description;
                SenderAgentId = model.SenderAgentId;
                SenderAgentPersonId = model.SenderAgentPersonId;
                SenderNumber = model.SenderNumber;
                SenderDate = model.SenderDate;
                Addressee = model.Addressee;
                AccessLevel = model.AccessLevel;
            }
            if (doc != null)
            {
                TemplateDocumentId = doc.TemplateDocumentId;
                IsHard = doc.IsHard;
                ExecutorPositionId = doc.ExecutorPositionId;
                DocumentDirection = doc.DocumentDirection;
                DocumentTypeId = doc.DocumentTypeId;
            }
        }

        public string DocumentDirectionName { get; set; }
        public string DocumentTypeName { get; set; }

        public string DocumentSubjectName { get; set; }
        public string RegistrationJournalName { get; set; }
        public string ExecutorPositionName { get; set; }
        public string ExecutorPositionAgentName { get; set; }

        public string SenderAgentName { get; set; }
        public string SenderAgentPersonName { get; set; }
        public string AccessLevelName { get; set; }

        public string GeneralInfo { get; set; }
    }
}