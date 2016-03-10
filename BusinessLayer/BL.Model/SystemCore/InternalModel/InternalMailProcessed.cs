using System;
using System.Collections.Generic;

namespace BL.Model.SystemCore.InternalModel
{
    /// <summary>
    /// Contains list of processed events.
    /// </summary>
    public class InternalMailProcessed
    {
        public InternalMailProcessed()
        {
            ProcessedEventIds = new List<int>();
            ProcessedDate = DateTime.Now;
        }

        /// <summary>
        /// List of events, which were processed
        /// </summary>
        public List<int> ProcessedEventIds { get; set; }

        /// <summary>
        /// Datetime 
        /// </summary>
        public DateTime ProcessedDate { get; set; }
    }
}