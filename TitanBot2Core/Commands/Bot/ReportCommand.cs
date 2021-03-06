﻿using Discord;
using System;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Bot
{
    [Description("Allows you to make suggestions and feature requests for me!")]
    public class ReportCommand : Command
    {
        [Call]
        [Usage("Sends a suggestion to my home guild.")]
        public async Task ReportAsync([Dense]string message)
        {
            var bugChannel = Context.Client.GetChannel(Configuration.Instance.BugChannel) as IMessageChannel;

            if (bugChannel == null)
            {
                await ReplyAsync("I could not find where I need to send the bug report! Please try again later.", ReplyType.Error);
                return;
            }

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"{Context.User.Username}#{Context.User.Discriminator}",
                    IconUrl = Context.User.GetAvatarUrl()
                },
                Timestamp = DateTime.Now,
                Color = System.Drawing.Color.IndianRed.ToDiscord()
            }
            .AddField("Bug report", message)
            .AddInlineField(Context.Guild?.Name ?? Context.User.Username, Context.Guild?.Id ?? Context.User.Id)
            .AddInlineField(Context.Channel.Name, Context.Channel.Id);
            await bugChannel.SendMessageSafeAsync("", embed: builder.Build());
            await ReplyAsync("Bug report sent", ReplyType.Success);
        }
    }
}
