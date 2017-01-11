using BL.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentTask : FrontDocumentInfo
    {
        public int Id { get; set; }

        public int PositionId { get; set; }
        public int PositionExecutorAgentId { get; set; }
        public int AgentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string AgentName { get; set; }
        public string PositionExecutorAgentName { get; set; }
        public string PositionExecutorNowAgentName { get; set; }
        public string PositionName { get; set; }
        public string PositionExecutorAgentPhoneNumber { get; set; }

        public bool IsFactExecutor { get; set; }
        /// <summary>
        /// ИД типа рассылки
        /// </summary>
        public EnumSendTypes ExecutorSendType { get; set; }
        public string ExecutorType { get; set; }
        public string ResponsibleExecutorPositionName { get; set; }
        public string ResponsibleExecutorPositionExecutorAgentName { get; set; }

    }
}
