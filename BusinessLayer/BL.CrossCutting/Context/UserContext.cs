using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.Users;
using System;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Enums;
using Ninject;
using Ninject.Parameters;

namespace BL.CrossCutting.Context
{
    /// <summary>
    /// Контекст пользователя - информационный класс, который НЕ должен содержать логику. Просто набор параметров, которые доступны на всех уровнях
    /// </summary>
    public class UserContext : IContext
    {
        private bool _isFormed;
        private int? _currentPositionId;
        private List<int> _currentPositionsIdList;
        private Dictionary<int, int> _currentPositionsAccessLevel;

        private DatabaseModel _currentDb;
        public Employee CurrentEmployee { get; set; }

        public UserContext()
        {
        }

        public UserContext(IContext ctx)
        {
            var def = ctx as UserContext;
            if (def != null)
            {
                try
                {
                    CurrentDB = new DatabaseModel
                    {
                        Id = ctx.CurrentDB.Id,
                        Name = ctx.CurrentDB.Name,
                        ServerType = ctx.CurrentDB.ServerType,
                        IntegrateSecurity = false,
                        Address = ctx.CurrentDB.Address,
                        DefaultDatabase = ctx.CurrentDB.DefaultDatabase,
                        UserName = ctx.CurrentDB.UserName,
                        UserPassword = ctx.CurrentDB.UserPassword,
                        DefaultSchema = ctx.CurrentDB.DefaultSchema,
                        ConnectionString = ctx.CurrentDB.ConnectionString,
                    };
                }
                catch (DatabaseIsNotSet)
                {
                    CurrentDB = null;
                }
                DbContext = ctx.DbContext;
                CurrentEmployee = new Employee
                {
                    AgentId = ctx.CurrentEmployee.AgentId,
                    LanguageId = ctx.CurrentEmployee.LanguageId,
                    Name = ctx.CurrentEmployee.Name,
                    Token = ctx.CurrentEmployee.Token,
                    UserId = ctx.CurrentEmployee.UserId,
                    ClientId = ctx.CurrentEmployee.ClientId,
                    ClientCode = ctx.CurrentEmployee.ClientCode,
                };

                ClientLicence = ctx.ClientLicence;
                IsChangePasswordRequired = ctx.IsChangePasswordRequired;
                IsFormed = ctx.IsFormed;
                LoginLogId = ctx.LoginLogId;
                LoginLogInfo = ctx.LoginLogInfo;
                UserName = ctx.UserName;

                DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", CurrentDB));

                try
                {
                    _currentPositionId = ctx.CurrentPositionId;
                }
                catch (UserContextIsNotDefined)
                {
                    _currentPositionId = null;
                }

                try
                {
                    CurrentPositionsIdList = ctx.CurrentPositionsIdList?.ToList();
                }
                catch (UserContextIsNotDefined)
                {
                    CurrentPositionsIdList = null;
                }

                try
                {
                    CurrentPositionsAccessLevel = ctx.CurrentPositionsAccessLevel?.ToDictionary(x => x.Key, x => x.Value);
                }
                catch (UserContextIsNotDefined)
                {
                    CurrentPositionsAccessLevel = null;
                }
            }
        }

        /// <summary>
        ///  Флаг TRUE если контекст сформирован и готов к работе
        /// </summary>
        public bool IsFormed
        {
            get { return _isFormed; }
            set { _isFormed = value; }
        }


        public List<int> CurrentPositionsIdList
        {
            get
            {
                if ((_currentPositionsIdList == null) || !_currentPositionsIdList.Any())
                {
                    throw new UserContextIsNotDefined();
                }
                return _currentPositionsIdList;
            }
            set
            {
                _currentPositionsIdList = value;
            }
        }

        public Dictionary<int, int> CurrentPositionsAccessLevel
        {
            get
            {
                if ((_currentPositionsAccessLevel == null) || !_currentPositionsAccessLevel.Any())
                {
                    throw new UserContextIsNotDefined();
                }
                return _currentPositionsAccessLevel;
            }
            set
            {
                _currentPositionsAccessLevel = value;
            }
        }

        public int CurrentPositionId
        {
            get
            {
                if (!_currentPositionId.HasValue)
                {
                    throw new UserContextIsNotDefined();
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
                    throw new UserContextIsNotDefined();
                }
                return CurrentEmployee.AgentId.GetValueOrDefault();
            }
        }

        public void SetCurrentPosition(int? position)
        {
            _currentPositionId = position;
        }

        public List<string> GetAccessFilterForFullText(string addFilter)
        {
            return CurrentPositionsAccessLevel
                        .Select(x => $"{x.Key}{FullTextFilterTypes.AccessPosition30}{addFilter}")
                        .Concat(CurrentPositionsAccessLevel.Where(x => x.Value <= (int)EnumAccessLevels.PersonallyAndReferents)
                        .Select(x => $"{x.Key}{FullTextFilterTypes.AccessPosition20}{addFilter}"))
                        .Concat(CurrentPositionsAccessLevel.Where(x => x.Value <= (int)EnumAccessLevels.Personally)
                        .Select(x => $"{x.Key}{FullTextFilterTypes.AccessPosition10}{addFilter}"))
                        .ToList();
        }

        public bool IsAdmin => false;

        public LicenceInfo ClientLicence { get; set; }

        public DatabaseModel CurrentDB
        {
            get
            {
                if (_currentDb == null)
                {
                    throw new DatabaseIsNotSet();
                }
                return _currentDb;
            }
            set
            {
                _currentDb = value;
            }
        }

        public int CurrentClientId
        {
            get
            {
                if (CurrentEmployee.ClientId <= 0)
                {
                    return 0;
                }
                return CurrentEmployee.ClientId;
            }
            set
            {
                CurrentEmployee.ClientId = value;
            }
        }

        public DateTime CreateDate { get; } = DateTime.UtcNow;
        public DateTime LastChangeDate { get; set; } = DateTime.UtcNow;
        public bool IsChangePasswordRequired { get; set; }

        public int? LoginLogId { get; set; }

        public string LoginLogInfo { get; set; }
        public IDmsDatabaseContext DbContext { get; set; }

        public string UserName { get; set; }
    }
}