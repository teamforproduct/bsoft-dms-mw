﻿namespace BL.Model.DocumentCore.Actions
{
    public class ChangeWorkStatus
    {
        public int DocumentId { get; set; }
        public bool IsInWork { get; set; }
        public string Description { get; set; }
    }
}
