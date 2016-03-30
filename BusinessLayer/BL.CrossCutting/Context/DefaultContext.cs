using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Model.Users;

namespace BL.CrossCutting.Context
{
    public class DefaultContext : IContext
    {
        private int? _currentPositionId;
        private List<int> _currentPositionsIdList;
        public DatabaseModel _currentDB;
        public Employee CurrentEmployee { get; set; }
        public List<int> CurrentPositionsIdList
        {
            get
            {
                //var position = CurrentPositions?.FirstOrDefault();
                if ((_currentPositionsIdList == null) || !_currentPositionsIdList.Any())
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

        public bool IsAdmin => false;

        public DatabaseModel CurrentDB
        {
            get
            {
                if (_currentDB == null)
                {
                    throw new DatabaseIsNotSet();
                }
                return _currentDB;
            }
            set
            {
                _currentDB = value;
            }
        }
    }
}