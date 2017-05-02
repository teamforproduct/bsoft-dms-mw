using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.Context;
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


        public UserContext() { }
        
        /// <summary>
        /// Создает новый контекст пользователя и новое ПОДКЛЮЧЕНИЕ к базе
        /// </summary>
        /// <param name="ctx"></param>
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
                Token = ctx.Token;
                DbContext = ctx.DbContext;
                User = new User
                {
                   Id = ctx.User.Id,
                   Name = ctx.User.Name,
                   Fingerprint = ctx.User.Fingerprint,
                   IsChangePasswordRequired = ctx.User.IsChangePasswordRequired,
                   LanguageId = ctx.User.LanguageId,
                };
                Employee = new Employee
                {
                    Id = ctx.Employee.Id,
                    LanguageId = ctx.Employee.LanguageId,
                    Name = ctx.Employee.Name,
                    IsActive = ctx.Employee.IsActive,
                    PositionExecutorsCount = ctx.Employee.PositionExecutorsCount,
                };
                Client = new Client
                {
                    Id = ctx.Client.Id,
                    Code = ctx.Client.Code,
                };

                ClientLicence = ctx.ClientLicence;
                IsFormed = ctx.IsFormed;
                LoginLogId = ctx.LoginLogId;
                LoginLogInfo = ctx.LoginLogInfo;

                // тут поднимается коннекшн к базе
                DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", CurrentDB));

                _currentPositionId = ctx.CurrentPositionDefined
                    ? ctx.CurrentPositionId
                    :(int?)null;

                CurrentPositionsIdList = ctx.CurrentPositionsIdListDefined
                    ? ctx.CurrentPositionsIdList?.ToList()
                    :null;

                CurrentPositionsAccessLevel = ctx.CurrentPositionsAccessLevelDefined
                    ? ctx.CurrentPositionsAccessLevel?.ToDictionary(x => x.Key, x => x.Value)
                    :null;
            }
        }

        /// <summary>
        /// Токен из авторизации
        /// </summary>
        public string Token { get; set; }


        public Employee Employee { get; set; }

        public Client Client { get; set; }

        public User User { get; set; }

        /// <summary>
        ///  Флаг TRUE если контекст сформирован и готов к работе
        /// </summary>
        public bool IsFormed
        {
            get { return _isFormed; }
            set { _isFormed = value; }
        }

        public bool CurrentPositionsIdListDefined => !((_currentPositionsIdList == null) || !_currentPositionsIdList.Any());

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

        public bool CurrentPositionsAccessLevelDefined => !((_currentPositionsAccessLevel == null) || !_currentPositionsAccessLevel.Any());

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

        public bool CurrentPositionDefined => _currentPositionId.HasValue;

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

        public bool CurrentAgentDefined => Employee?.Id != null;

        public int CurrentAgentId
        {
            get
            {
                if (Employee?.Id == null)
                {
                    throw new UserContextIsNotDefined();
                }
                return Employee.Id.GetValueOrDefault();
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
                if (_currentDb == null) throw new DatabaseIsNotSet();
                return _currentDb;
            }
            set
            {
                _currentDb = value;
            }
        }
        private DatabaseModel _currentDb;

        public DateTime CreateDate { get; } = DateTime.UtcNow;
        public DateTime LastChangeDate { get; set; } = DateTime.UtcNow;

        public int? LoginLogId { get; set; }

        public string LoginLogInfo { get; set; }
        public IDmsDatabaseContext DbContext { get; set; }

    }
}