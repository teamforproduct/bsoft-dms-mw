namespace BL.Model.Enums
{
    /// <summary>
    /// Список экшенов по админке
    /// </summary>
    public enum EnumAdminActions
    {

        #region [+] Roles ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddRole = 700001,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyRole = 700005,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteRole = 700009,
        #endregion

        #region [+] RoleActions ...
        SetRolePermission = 701000,
        /// <summary>
        /// Добавить
        /// </summary>
        SetRolePermissionByModuleFeature = 701001,
        /// <summary>
        /// Изменить
        /// </summary>
        SetRolePermissionByModule = 701002,

        SetRolePermissionByModuleAccessType = 701003,
        #endregion

        #region [+] PositionRole ...
        SetPositionRole = 702000,
        /// <summary>
        /// Дублировать 
        /// </summary>
        DuplicatePositionRoles = 702002,
        #endregion

        #region [+] UserRoles ...
        SetUserRole = 703000,
        /// <summary>
        /// Добавить
        /// </summary>
        SetUserRoleByAssignment = 703001,
        /// <summary>
        /// Изменить
        /// </summary>
        //ModifyUserRole = 703005,
        /// <summary>
        /// Удалить 
        /// </summary>
        //DeleteUserRoleByUser = 703007,
        //DeleteUserRoleByPositionExecutor = 703008,
        // DeleteUserRole = 703009,
        #endregion

        #region [+] Subordinations ...
        ///// <summary>
        ///// Добавить
        ///// </summary>
        //AddSubordination = 704001,
        ///// <summary>
        ///// Изменить
        ///// </summary>
        //ModifySubordination = 704005,
        ///// <summary>
        ///// Удалить 
        ///// </summary>
        //DeleteSubordination = 704009,

        /// <summary>
        /// Совокупность действий по управлению субординацией
        /// </summary>
        SetSubordination = 704001,

        /// <summary>
        /// Копирование рассылки
        /// </summary>
        DuplicateSubordinations = 704002,
        /// <summary>
        /// Совокупность действий по управлению субординацией для всего подразделения
        /// </summary>
        SetSubordinationByDepartment = 704003,

        /// <summary>
        /// Совокупность действий по управлению субординацией для всей компании
        /// </summary>
        SetSubordinationByCompany = 704004,

        /// <summary>
        /// Совокупность действий по управлению субординацией для всей компании
        /// </summary>
        SetDefaultSubordination = 704005,

        /// <summary>
        /// Разрешить все
        /// </summary>
        SetAllSubordination = 704006,
        #endregion

        #region [+] RegistrationJournalPositions ...

        /// <summary>
        /// Совокупность действий по управлению доступом к журналам
        /// </summary>
        SetJournalAccess = 706000,

        /// <summary>
        /// Копирование доступа от журнала к журналу
        /// </summary>
        DuplicateJournalAccess_Journal = 706001,
        /// <summary>
        /// Копирование доступа от должнсости к должности
        /// </summary>
        DuplicateJournalAccess_Position = 706002,


        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByDepartment_Journal = 706003,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByDepartment_Position = 706004,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByCompany_Journal = 706005,
        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessByCompany_Position = 706006,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessDefault_Journal = 706007,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessDefault_Position = 706008,


        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessAll_Journal = 706009,

        /// <summary>
        /// Управлению доступом к журналам
        /// </summary>
        SetJournalAccessAll_Position = 706010,

        #endregion

        #region [+] DepartmentAdmin ...

        /// <summary>
        /// Добавить администратора подразделения
        /// </summary>
        AddDepartmentAdmin = 210003,

        /// <summary>
        /// Удалить администратора подразделения
        /// </summary>
        DeleteDepartmentAdmin = 210002,

        #endregion

        #region [+] Action over AgentUser
        //ChangePassword = 221001,
        //ChangeLockout = 221002,
        //KillSessions = 221003,
        ChangeLogin = 221004,
        //MustChangePassword = 221005,

        #endregion

        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!

    }
}