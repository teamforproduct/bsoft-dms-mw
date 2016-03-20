namespace BL.Model.AutoPlan
{
    /// <summary>
    /// Setting for autoplan service
    /// </summary>
    public class AutoPlanSettings
    {
        /// <summary>
        /// how often we should check documents to start autoplan
        /// </summary>
        public int TimeToUpdate { get; set; }

        /// <summary>
        /// Key for searching DB
        /// </summary>
        public string DatabaseKey { get; set; }
    }
}