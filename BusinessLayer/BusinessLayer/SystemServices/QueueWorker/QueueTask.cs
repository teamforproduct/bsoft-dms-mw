using System;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Logic.DocumentCore.Interfaces;
using BL.Model.Enums;

namespace BL.Logic.SystemServices.QueueWorker
{
    public class QueueTask:IQueueTask
    {
        readonly ICommand _command;
        readonly Action _action;

        public QueueTask(ICommand command)
        {
            _command = command;
        }

        public QueueTask(Action action)
        {
            _action = action;
        }

        public bool CanExecute()
        {
            if (_command != null)
            {
                return _command.CanExecute();
            }
            return true;
        }

        public void Execute()
        {
            if (_command != null)
            {
                var cmdService = DmsResolver.Current.Get<ICommandService>();
                cmdService.ExecuteCommand(_command);
            }
            else
            {
                _action?.Invoke();
            }
        }

        public EnumWorkStatus Status { get; set; }
        public string StatusDescription { get; set; }
        public string Name { get; set; }
    }
}