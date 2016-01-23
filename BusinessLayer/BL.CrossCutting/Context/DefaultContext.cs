using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Users;

namespace BL.CrossCutting.Context
{
    public class DefaultContext :IContext
    {
        public Employee CurrentEmployee { get; set; }
        public List<Position> CurrentPosition { get; set; }
        public DatabaseModel CurrentDB { get; set; }
    }
}