using BL.Model.Common;
using BL.Model.AdminCore.IncomingModel;

namespace BL.Model.AdminCore.InternalModel
{
    public class InternalAdminSubordination : LastChangeInfo
    {
        public InternalAdminSubordination()
        { }

        public InternalAdminSubordination(ModifyAdminSubordination model)
        {
            Id = model.Id;
            SourcePositionId = model.SourcePositionId;
            TargetPositionId = model.TargetPositionId;
            SubordinationTypeId = (int)model.SubordinationTypeId;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Должность руководителя
        /// </summary>
        public int SourcePositionId { get; set; }

        /// <summary>
        /// Должность исполнителя
        /// </summary>
        public int TargetPositionId { get; set; }

        /// <summary>
        /// Тип переписки
        /// </summary>
        public int SubordinationTypeId { get; set; }

    }
}