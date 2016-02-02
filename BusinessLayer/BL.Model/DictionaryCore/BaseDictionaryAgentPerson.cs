using BL.Model.Enums;
using System;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryAgentPerson
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string Name { get; set; }
        public Nullable<int> PersonAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string AgentName { get; set; }
        public string PersonAgentName { get; set; }
    }
}