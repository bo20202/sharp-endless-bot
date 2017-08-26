using System.Collections.Generic;

namespace EndlessConfiguration.Models.Backend
{
    public class Backend
    {
        public string Address { get; set; }
        public Dictionary<string, Action> Controller { get; set; }
    }

    public class Action
    {
        public Dictionary<string, string> ActionNameDictionary { get; set; }
    }
}