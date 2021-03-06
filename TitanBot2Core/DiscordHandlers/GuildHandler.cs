﻿using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using TitanBot2.Common;

namespace TitanBot2.DiscordHandlers
{
    public class GuildHandler : HandlerBase
    {
        public override async Task Install(BotDependencies args)
        {
            await base.Install(args);

            Client.GuildAvailable += HandleAvailableAsync;
            Client.GuildUnavailable += HandleUnavailableAsync;
            Client.GuildUpdated += HandleUpdatedAsync;
            Client.JoinedGuild += HandleJoinAsync;
            Client.LeftGuild += HandleLeftAsync;
            Client.RoleCreated += HandleRoleCreateAsync;
            Client.RoleDeleted += HandleRoleDeletedAsync;
            Client.RoleUpdated += HandleRoleUpdateddAsync;

            await BotClient.Logger.Log(new BotLog(LogType.Handler, LogSeverity.Info, "Installed successfully", "Guild"));
        }

        public override async Task Uninstall()
        {
            Client.GuildAvailable -= HandleAvailableAsync;
            Client.GuildUnavailable -= HandleUnavailableAsync;
            Client.GuildUpdated -= HandleUpdatedAsync;
            Client.JoinedGuild -= HandleJoinAsync;
            Client.LeftGuild -= HandleLeftAsync;
            Client.RoleCreated -= HandleRoleCreateAsync;
            Client.RoleDeleted -= HandleRoleDeletedAsync;
            Client.RoleUpdated -= HandleRoleUpdateddAsync;

            await BotClient.Logger.Log(new BotLog(LogType.Handler, LogSeverity.Info, "Uninstalled successfully", "Guild"));
            await base.Uninstall();
        }

        private async Task HandleAvailableAsync(SocketGuild guild)
        {
            await Database.Guilds.EnsureExists(guild.Id);
        }

        private async Task HandleUnavailableAsync(SocketGuild guild)
        {

        }

        private async Task HandleUpdatedAsync(SocketGuild before, SocketGuild after)
        {

        }

        private async Task HandleJoinAsync(SocketGuild guild)
        {
            await BotClient.Logger.Log(new BotLog(LogType.Handler, LogSeverity.Info, $"Joined {guild.Name} ({guild.Id})", "Guild"));
            await Database.Guilds.EnsureExists(guild.Id);
        }

        private async Task HandleLeftAsync(SocketGuild guild)
        {

        }

        private async Task HandleRoleCreateAsync(SocketRole role)
        {

        }

        private async Task HandleRoleDeletedAsync(SocketRole role)
        {

        }

        private async Task HandleRoleUpdateddAsync(SocketRole oldRole, SocketRole newRole)
        {

        }
    }
}
