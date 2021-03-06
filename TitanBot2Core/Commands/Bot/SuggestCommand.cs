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
    public class SuggestCommand : Command
    {
        [Call]
        [Usage("Sends a suggestion to my home guild.")]
        public async Task SuggestAsync([Dense]string message)
        {
            var suggestionChannel = Context.Client.GetChannel(Configuration.Instance.SuggestChannel) as IMessageChannel;

            if (suggestionChannel == null)
            {
                await ReplyAsync("I could not find where I need to send the suggestion! Please try again later.", ReplyType.Error);
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
                Color = System.Drawing.Color.ForestGreen.ToDiscord()
            }
            .AddField("Suggestion", message)
            .AddInlineField(Context.Guild?.Name ?? Context.User.Username, Context.Guild?.Id ?? Context.User.Id)
            .AddInlineField(Context.Channel.Name, Context.Channel.Id);
            await suggestionChannel.SendMessageSafeAsync("", embed: builder.Build());
            await ReplyAsync("Suggestion sent", ReplyType.Success);
        }
    }
}
