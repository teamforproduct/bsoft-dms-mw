﻿using System;
using BL.Model.Enums;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontTemplateRestrictedSendList
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? PositionId { get; set; }
        public EnumAccessLevels? AccessLevel { get; set; }
        public string PositionName { get; set; }
        public string PositionExecutorAgentName { get; set; }
        public string AccessLevelName { get; set; }
    }
}
