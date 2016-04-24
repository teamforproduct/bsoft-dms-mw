namespace BL.Model.AdminCore
{
    /// <summary>
    /// описывает доступные пользователю роли
    /// </summary>
    public class FrontAdminUserRole
    {
        public int RolePositionId { get; set; }
        public string RolePositionName { get; set; }
        public string RolePositionExecutorAgentName { get; set; }
        public int NewEventsCount { get; set; }
    }
}
