using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

namespace BL.Model.AdminCore.FrontModel
{
    public class FrontDIPRegistrationJournalPositions 
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionName { get; set; }

        public bool IsViewing { get; set; }

        public bool IsRegistration { get; set; }
    }
}