﻿using Discord;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Commands.Data;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;
using TitanBot2.Services.Database.Tables;
using TitanBot2.Services.Scheduler;
using TitanBot2.TimerCallbacks;

namespace TitanBot2.Commands.Clan
{
    [Description("Used for Titan Lord timers and management")]
    [DefaultPermission(8)]
    [RequireContext(ContextType.Guild)]
    [Alias("TL", "Boss")]
    class TitanLordCommand : Command
    {
        [Call("In")]
        [Usage("Sets a Titan Lord timer running for the given period.")]
        private Task TitanLordInAsync(TimeSpan time)
        {
            lock (_guildCommandLock)
            {
                LockedTitanLordIn(time).Wait();
            }

            return Task.CompletedTask;
        }

        [Call("Dead")]
        [Usage("Sets a Titan Lord timer running for 6 hours.")]
        private Task TitanLordDead()
            => TitanLordInAsync(new TimeSpan(6, 0, 0));

        private async Task LockedTitanLordIn(TimeSpan time)
        {
            var roundsRunning = (await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordRound)).Count();
            var timersRunning = (await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordNow)).Count();

            await CompleteExisting();

            if (roundsRunning > 0 && timersRunning == 0)
                await NewBoss(time);

            var timeNow = DateTime.Now;

            var tickTimer = new Timer
            {
                GuildId = Context.Guild.Id,
                UserId = Context.User.Id,
                ChannelId = Context.Channel.Id,
                MessageId = Context.Message.Id,
                Callback = EventCallback.TitanLordTick,
                From = timeNow,
                SecondInterval = 10,
                To = timeNow.Add(time)
            };
            var nowTimer = new Timer
            {
                GuildId = Context.Guild.Id,
                UserId = Context.User.Id,
                ChannelId = Context.Channel.Id,
                MessageId = Context.Message.Id,
                Callback = EventCallback.TitanLordNow,
                From = timeNow.Add(time),
                SecondInterval = 1,
                To = timeNow.Add(time)
            };
            var roundTimer = new Timer
            {
                GuildId = Context.Guild.Id,
                UserId = Context.User.Id,
                ChannelId = Context.Channel.Id,
                MessageId = Context.Message.Id,
                Callback = EventCallback.TitanLordRound,
                From = timeNow.Add(time).AddHours(1),
                SecondInterval = 60 * 60 - 30,
                To = timeNow.Add(time).AddDays(1)
            };
            
            var tlChannel = Context.Channel;
            if (Context.GuildData.TitanLord?.Channel != null)
                tlChannel = Context.Guild.GetTextChannel(Context.GuildData.TitanLord.Channel.Value) ?? tlChannel;

            var message = await tlChannel.SendMessageSafeAsync("Loading Timer...\n*if this takes longer than 10s please PM Titansmasher ASAP*");

            if (Context.GuildData.TitanLord.PinTimer && Context.Guild.GetUser(Context.Client.CurrentUser.Id).GuildPermissions.Has(GuildPermission.ManageMessages))
                await message.PinAsync();

            var custArgs = new JObject();

            custArgs.Add(TitanLordCallbacks.timerMessageId, message.Id);
            custArgs.Add(TitanLordCallbacks.timerMessageChannelId, tlChannel.Id);

            tickTimer.CustArgs = custArgs;
            nowTimer.CustArgs = custArgs;

            await Context.TimerService.AddTimers(new Timer[] { tickTimer, nowTimer, roundTimer });

            await ReplyAsync($"Started a timer for **{time}**", ReplyType.Success);
        }

        [Call("Now")]
        [Usage("Alerts everyone that the Titan Lord is ready to be killed right now")]
        private async Task TitanLordNowAsync()
        {
            await CompleteExisting();

            var time = DateTime.Now;

            var timer = new Timer
            {
                GuildId = Context.Guild.Id,
                UserId = Context.User.Id,
                ChannelId = Context.Channel.Id,
                MessageId = Context.Message.Id,
                Callback = EventCallback.TitanLordRound,
                From = time.AddHours(1),
                SecondInterval = 60 * 60 - 30,
                To = time.AddDays(1)
            };
            var nowTimer = new Timer
            {
                GuildId = Context.Guild.Id,
                UserId = Context.User.Id,
                ChannelId = Context.Channel.Id,
                MessageId = Context.Message.Id,
                Callback = EventCallback.TitanLordNow,
                From = DateTime.Now,
                SecondInterval = 1,
                To = DateTime.Now
            };

            await Context.Database.Timers.Add(nowTimer);
            await Context.Database.Timers.Add(timer);

            await ReplyAsync("Ill let everyone know!", ReplyType.Success);
        }

        [Call("When")]
        [Usage("Gets the time until the Titan Lord is ready to be killed")]
        private async Task TitanLordWhenAsync()
        {
            var activeTimer = await Context.Database.Timers.GetLatest(Context.Guild.Id, EventCallback.TitanLordNow);

            if (activeTimer == null)
                await ReplyAsync("There are no timers currently running", ReplyType.Error);
            else
                await ReplyAsync($"There will be a TitanLord in {(activeTimer.To.Value - DateTime.Now).Beautify()}", ReplyType.Success);
        }

        [Call("Info")]
        [Usage("Gets information about the clans current level")]
        private Task TitanLordInfoAsync()
            => ReplyAsync("", embed: ClanStatsCommand.StatsBuilder(Context.Client.CurrentUser, Context.GuildData.TitanLord.CQ, 4000, 500, new int[] { 20, 30, 40, 50 }).Build());

        [Call("Stop")]
        [Usage("Stops any currently running timers.")]
        private async Task TitanLordStopAsync()
        {
            await CompleteExisting();
            await ReplyAsync("Stopped all existing timers", ReplyType.Success);
        }

        private async Task CompleteExisting(bool titanLordTicks = true, bool titanLordRounds = true, bool titanLordNows = true)
        {
            var existingTimers = new List<Timer>();

            if (titanLordTicks)
                existingTimers = existingTimers.Concat(await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordTick)).ToList();

            foreach (var timer in existingTimers)
            {
                var tickMessageId = (ulong?)timer.CustArgs[TitanLordCallbacks.timerMessageId];
                var tickMessageChannelId = (ulong?)timer.CustArgs[TitanLordCallbacks.timerMessageChannelId];

                if (tickMessageId == null || tickMessageChannelId == null)
                    continue;

                await Context.Guild.DeleteMessage(tickMessageChannelId.Value, tickMessageId.Value);
            }

            if (titanLordRounds)
                existingTimers = existingTimers.Concat((await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordRound)).ToList()).ToList();
            if (titanLordNows)
                existingTimers = existingTimers.Concat((await Context.Database.Timers.Get(guildid: Context.Guild.Id, callback: EventCallback.TitanLordNow)).ToList()).ToList();

            await Context.Database.Timers.Complete(existingTimers);
        }

        private async Task NewBoss(TimeSpan time)
        {
            Context.GuildData.TitanLord.CQ += 1;

            await Context.Database.Guilds.Update(Context.GuildData);

            var bossHp = Calculator.TitanLordHp(Context.GuildData.TitanLord.CQ);
            var clanBonus = Calculator.ClanBonus(Context.GuildData.TitanLord.CQ);
            var advStart = Calculator.AdvanceStart(Context.GuildData.TitanLord.CQ);

            var latestTimer = await Context.Database.Timers.GetLatest(Context.Guild.Id, EventCallback.TitanLordNow, true);

            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    IconUrl = Res.Emoji.Information_source,
                    Name = "Titan Lord data updated!"
                },
                ThumbnailUrl = "https://cdn.discordapp.com/attachments/275257967937454080/308047011289235456/emoji.png",
                Color = System.Drawing.Color.DarkOrange.ToDiscord(),
                Timestamp = DateTime.Now,
            };

            builder.AddField("New Clan Quest", Context.GuildData.TitanLord.CQ);
            builder.AddField("New bonus", clanBonus.Beautify());
            builder.AddField("Next Titan Lord HP", bossHp.Beautify());
            builder.AddField("Time to kill", (DateTime.Now.Add(time).AddHours(-6) - latestTimer.To).Value.Beautify());


            var tlChannel = Context.Channel.Id;
            if (Context.GuildData.TitanLord?.Channel != null)
                tlChannel = Context.GuildData.TitanLord.Channel.Value;

            await TrySend(tlChannel, "", embed: builder.Build());
        }
    }
}
