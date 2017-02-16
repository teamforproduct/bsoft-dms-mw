using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalRegistrationJournalPosition : LastChangeInfo
    {
        public InternalRegistrationJournalPosition()
        { }

        public InternalRegistrationJournalPosition(SetJournalAccess model)
        {
            Id = model.Id;
            PositionId = model.PositionId;
            RegistrationJournalId = model.RegistrationJournalId;
            RegJournalAccessTypeId = (int)model.RegJournalAccessTypeId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id Должности
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Id журнала регистрации
        /// </summary>
        public int RegistrationJournalId { get; set; }

        /// <summary>
        /// Тип доступа к журналу
        /// </summary>
        public int RegJournalAccessTypeId { get; set; }

    }
}