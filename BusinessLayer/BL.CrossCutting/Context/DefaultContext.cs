using System.Collections.Generic;
using BL.Model.Database;
using BL.Model.Users;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.Exception;

namespace BL.Logic.Context
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
                if ((_currentPositionsIdList == null)||!_currentPositionsIdList.Any())
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

        public int CurrentPositionId
        {
            get
            {
                if (!_currentPositionId.HasValue)
                {
                    throw new UserPositionIsNotDefined();
                }
                return _currentPositionId.Value;
            }
        }

        

        public int CurrentAgentId
        {
            get
            {
                if (CurrentEmployee?.AgentId == null)
                {
                    throw new UserNameIsNotDefined();
                }
                return CurrentEmployee.AgentId.GetValueOrDefault();
            }
        }

        public void SetCurrentPosition(int? position)
        {
            _currentPositionId = position;
        }

        public DatabaseModel CurrentDB { get; set; }
    }
}