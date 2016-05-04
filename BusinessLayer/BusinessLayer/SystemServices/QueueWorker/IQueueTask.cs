using BL.Model.Enums;

namespace BL.Logic.SystemServices.QueueWorker
{
    public interface IQueueTask
    {
        bool CanExecute();
        void Execute();
        EnumWorkStatus Status { get; set; }
        string StatusDescription { get; set; }
        string Name { get; set; }
    }
}