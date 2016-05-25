using BL.Model.Users;

namespace BL.Model.DocumentCore.Actions
{
    /// <summary>
    /// Модель создания копий бумажных носителей
    /// </summary>
    public class PlanMovementPaper: PaperEvent
    {
        /// <summary>
        /// ИД должности, кому направляется БН
        /// </summary>
        public int TargetPositionId { get; set; }
    }
}
