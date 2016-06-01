﻿using System;
using BL.CrossCutting.Interfaces;
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
                _command.Execute();
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