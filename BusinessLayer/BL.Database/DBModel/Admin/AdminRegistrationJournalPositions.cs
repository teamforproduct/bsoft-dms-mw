using BL.Database.DBModel.Dictionary;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Admin
{
    public class AdminRegistrationJournalPositions
    {
        public int Id { get; set; }

        [Index("IX_JournalPositionType", 1, IsUnique = true)]
        public int PositionId { get; set; }

        [Index("IX_JournalPositionType", 2, IsUnique = true)]
        [Index("IX_RegJournalId", 1)]
        public int RegJournalId { get; set; }

        [Index("IX_JournalPositionType", 3, IsUnique = true)]
        [Index("IX_RegJournalAccessTypeId", 1)]
        public int RegJournalAccessTypeId { get; set; }


        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }

        [ForeignKey("RegJournalId")]
        public virtual DictionaryRegistrationJournals RegistrationJournal { get; set; }

        [ForeignKey("RegJournalAccessTypeId")]
        public virtual DicRegJournalAccessTypes RegJournalAccessType { get; set; }
    }
}
