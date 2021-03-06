﻿using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.GuildSpecific.Singularity
{
    [Description("Demotes a user down the ranks, potentially even out of the clan")]
    [RequireGuild(307803032534646785)]
    [DefaultPermission(8)]
    class DemoteCommand : Command
    {
        private ulong[] _roleOrder = new ulong[]
        {
            307806171798962177,
            307805963295916032,
            307806447402614785,
            307806702151925760,
            307807022903197698,
            312177555379585024,
        };

        [Call]
        [Usage("Demotes the given user")]
        async Task Demote([Dense]SocketGuildUser user)
        {
            var callingUser = Context.User as SocketGuildUser;
            if (callingUser == null)
            {
                await ReplyAsync("An error occured, please try again later.", ReplyType.Error);
                return;
            }

            var userTopRole = callingUser.Roles.OrderBy(r => r.Position).FirstOrDefault(r => _roleOrder.Contains(r.Id));
            var targetTopRole = user.Roles.OrderBy(r => r.Position).FirstOrDefault(r => _roleOrder.Contains(r.Id));

            if (userTopRole == null)
            {
                await ReplyAsync("You do not have a member rank here!", ReplyType.Error);
                return;
            }

            if (targetTopRole == null)
            {
                await ReplyAsync("They do not have a member rank here! Please give them one of the member roles to use this command on them.", ReplyType.Error);
                return;
            }

            if (userTopRole.Position <= targetTopRole.Position)
            {
                await ReplyAsync("You cannot demote someone who is above or equal to you", ReplyType.Error);
                return;
            }

            var roles = Context.Guild.Roles.Where(r => _roleOrder.Contains(r.Id)).OrderBy(r => r.Position);
            var demoRole = roles.LastOrDefault(r => r.Position < targetTopRole.Position);

            if (demoRole == null)
            {
                await ReplyAsync("There is no lower rank for me to demote them to!", ReplyType.Error);
                return;
            }

            await user.RemoveRolesAsync(roles.Take(roles.Count() - 1).Where(r => user.Roles.Contains(r)));
            await user.AddRoleAsync(demoRole);

            await ReplyAsync($"Demoted {user.Username} to {demoRole.Name}");
        }
    }
}

