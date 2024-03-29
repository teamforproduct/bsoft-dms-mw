﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryStandartSendLists
    {
        public DictionaryStandartSendLists()
        {
            this.StandartSendListContents = new HashSet<DictionaryStandartSendListContents>();
        }

        public int Id { get; set; }
        [Index("IX_PositionName", 3, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(400)]
        [Index("IX_PositionName", 2, IsUnique = true)]
        public string Name { get; set; }
        [Index("IX_PositionName", 1, IsUnique = true)]
        public Nullable<int> PositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("StandartSendListId")]
        public virtual ICollection<DictionaryStandartSendListContents> StandartSendListContents { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
