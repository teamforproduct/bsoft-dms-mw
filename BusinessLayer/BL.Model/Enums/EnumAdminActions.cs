namespace BL.Model.Enums
{
    /// <summary>
    /// Список экшенов по админке
    /// </summary>
    //pss назначить номера действиям
    public enum EnumAdminActions
    {

        #region [+] Roles ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddRole,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyRole,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteRole,
        #endregion

        #region [+] RoleActions ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddRoleAction,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyRoleAction,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteRoleAction,
        #endregion

        #region [+] PositionRole ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddPositionRole,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyPositionRole,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeletePositionRole ,
        #endregion

        #region [+] UserRoles ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddUserRole,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifyUserRole,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteUserRole,
        #endregion

        #region [+] Subordinations ...
        /// <summary>
        /// Добавить
        /// </summary>
        AddSubordination,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifySubordination,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteSubordination,
        #endregion

    }
}