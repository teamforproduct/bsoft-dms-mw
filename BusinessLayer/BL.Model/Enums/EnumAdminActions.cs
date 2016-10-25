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
        /// <summary>
        /// Добавить
        /// </summary>
        AddRoleAction = 701001,
        /// <summary>
        /// Изменить
        /// </summary>
        //ModifyRoleAction = 701005,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteRoleAction = 701009,
        #endregion

        #region [+] PositionRole ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddPositionRole = 702001,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyPositionRole = 702005,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeletePositionRole = 702009,

        /// <summary>
        /// Дублировать 
        /// </summary>
        DuplicatePositionRoles = 702002,
        #endregion

        #region [+] UserRoles ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddUserRole = 703001,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyUserRole = 703005,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteUserRole = 703009,
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

        #region [+] DepartmentAdmin ...

        /// <summary>
        /// Добавить администратора подразделения
        /// </summary>
        AddDepartmentAdmin = 705001,

        /// <summary>
        /// Удалить администратора подразделения
        /// </summary>
        DeleteDepartmentAdmin = 705002,

        #endregion

        // При добавлении действия не забудь добавить действие в ImportData: BL.Database.DatabaseContext.DmsDbImportData!!!

    }
}