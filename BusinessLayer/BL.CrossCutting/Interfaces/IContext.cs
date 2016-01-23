using BL.Model.Database;
using BL.Model.Users;
using System.Collections.Generic;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        Employee CurrentEmployee { get; set; }
        List<Position> CurrentPosition { get; set; }
        DatabaseModel CurrentDB { get; set; }
    }
}