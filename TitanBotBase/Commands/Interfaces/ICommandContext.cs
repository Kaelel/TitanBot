﻿using Discord;
using Discord.WebSocket;
using TitanBotBase.Settings;

namespace TitanBotBase.Commands
{
    public interface ICommandContext
    {
        IUser Author { get; }
        IMessageChannel Channel { get; }
        DiscordSocketClient Client { get; }
        IGuild Guild { get; }
        GuildSettings GuildData { get; }
        IUserMessage Message { get; }
        int ArgPos { get; }
        string Prefix { get; }
        string CommandText { get; }
        CommandInfo Command { get; }
        bool IsCommand { get; }
        bool ExplicitPrefix { get; }

        void CheckCommand(ICommandService commandService, string defaultPrefix);
        string[] SplitArguments(out (string Key, string Value)[] flags, int? maxLength = null, int? densePos = null);
    }
}