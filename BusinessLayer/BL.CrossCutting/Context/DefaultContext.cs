using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Users;
using System.Linq;
using BL.Model.Exception;

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
                    throw new UserPositionIsNotDefined();
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
                    throw new UserNameIsNotDefined();
                }
                return CurrentEmployee.AgentId.GetValueOrDefault();
            }
        }

        public DatabaseModel CurrentDB { get; set; }
    }
}