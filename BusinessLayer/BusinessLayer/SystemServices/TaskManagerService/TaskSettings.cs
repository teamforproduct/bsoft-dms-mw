using System;
using System.Threading;
using BL.CrossCutting.Interfaces;
using BL.Model.Context;

namespace BL.Logic.SystemServices.TaskManagerService
{
    public class TaskSettings
    {
        public DatabaseModelForAdminContext DatabaseModel { get; set; }
        public int PeriodInMinute { get; set; }
        public int Id { get; set; }
        public Timer TaskTimer { get; set; }
        public Action<IContext, object> TaskAction { get; set; }
        public IContext Context { get; set; }
        public object Parameter { get; set; }
    }
}