﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services;
using TitanBot2.Services.Database;
using TitanBot2.Services.Scheduler;

namespace TitanBot2.Common
{
    public class BotDependencies
    {
        public BotClient BotClient { get; set; }
        public DiscordSocketClient Client { get; set; }
        public BotDatabase Database { get; set; }
        public TimerService TimerService { get; set; }
        public Logger Logger { get; set; }
        public CachedWebClient WebClient { get; set; }
        public TT2DataService TT2DataService { get; set; }
        public ulong SuggestionChannelID { get; set; }
        public ulong BugChannelID { get; set; }
    }
}
