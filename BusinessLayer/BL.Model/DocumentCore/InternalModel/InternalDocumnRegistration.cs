using System;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumnRegistration
    {
        public int? RegistrationJournalId { get; set; }
        public string RegistrationJournalIndex { get; set; }
        public string RegistrationJournalDepartmentCode { get; set; }

        public string InitiativeRegistrationFullNumber { get; set; }

        public string InitiativeRegistrationNumberPrefix { get; set; }

        public string InitiativeRegistrationNumberSuffix { get; set; }
        public int? InitiativeRegistrationNumber { get; set; }
        public string ExecutorPositionDepartmentCode { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string SubscriptionsPositionDepartmentCode { get; set; }

        public string CurrentPositionDepartmentCode { get; set; }

        public string InitiativeRegistrationSenderNumber { get; set; }

    }
}
