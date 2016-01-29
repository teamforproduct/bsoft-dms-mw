using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Users;
using System.Linq;

namespace BL.CrossCutting.Context
{
    public class DefaultContext :IContext
    {
        public Employee CurrentEmployee { get; set; }
        public List<Position> CurrentPosition { get; set; }

        public int CurrentPositionId
        {
            get
            {
                var position = CurrentPosition.FirstOrDefault();
                if (position == null)
                {
                    throw new System.Exception();
                }
                return position.PositionId;
            }
        }

        public int CurrentAgentId
        {
            get
            {
                if (!CurrentEmployee.AgentId.HasValue)
                {
                    throw new System.Exception();
                }
                return CurrentEmployee.AgentId.GetValueOrDefault();
            }
        }

        public DatabaseModel CurrentDB { get; set; }
    }
}