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
        private int? _currentPositionId;
        private List<int> _currentPositionsIdList;
        public Employee CurrentEmployee { get; set; }
        public List<int> CurrentPositionsIdList
        {
            get
            {
                //var position = CurrentPositions?.FirstOrDefault();
                if ((_currentPositionsIdList == null)||(_currentPositionsIdList.Count()==0))
                {
                    throw new UserPositionIsNotDefined();
                }
                return _currentPositionsIdList;
            }
            set
            {
                _currentPositionsIdList = value;
            }
        }

        public int? CurrentPositionId
        {
            get
            {
                if (!_currentPositionId.HasValue)
                {
                    throw new UserPositionIsNotDefined();
                }
                return _currentPositionId;
            }
            set
            {
                _currentPositionId = value;
            }
        }

        public int CurrentAgentId
        {
            get
            {
                if (CurrentEmployee == null || !CurrentEmployee.AgentId.HasValue)
                {
                    throw new UserNameIsNotDefined();
                }
                return CurrentEmployee.AgentId.GetValueOrDefault();
            }
        }

        public DatabaseModel CurrentDB { get; set; }
    }
}