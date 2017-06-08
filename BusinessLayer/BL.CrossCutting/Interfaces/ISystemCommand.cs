﻿using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface ISystemCommand:ICommand
    {
        void InitializeCommand(EnumActions action, IContext ctx);
        void InitializeCommand(EnumActions action, IContext ctx, object param);
        EnumActions CommandType { get; }
    }
}