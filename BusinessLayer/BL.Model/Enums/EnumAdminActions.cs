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
        ModifyRoleAction = 701005,
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
        /// <summary>
        /// Добавить
        /// </summary>
        AddSubordination = 704001,
        /// <summary>
        /// Изменить
        /// </summary>
        ModifySubordination = 704005,
        /// <summary>
        /// Удалить 
        /// </summary>
        DeleteSubordination = 704009
        #endregion

    }
}