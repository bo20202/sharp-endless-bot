﻿using System.Collections.Generic;
using BotCore.Interfaces;
using EndlessConfiguration;
using EndlessConfiguration.Models;

namespace BotCore.Modules
{
    public class UpdateService : IUpdateService 
    {
        public Dictionary<Server, bool> Updating { get; private set; }
        public void Update(Server server)
        {
            
        }
    }
}