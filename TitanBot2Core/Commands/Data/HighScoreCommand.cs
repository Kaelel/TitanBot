﻿using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;

namespace TitanBot2.Commands.Data
{
    [Description("Shows data from the high score sheet, which can be found [here](https://docs.google.com/spreadsheets/d/13hsvWaYvp_QGFuQ0ukcgG-FlSAj2NyW8DOvPUG3YguY/pubhtml?gid=4642011cYS8TLGYU)\nAll credit to <@261814131282149377>, <@169180650203512832> and <@169915601496702977> for running the sheet!")]
    [Alias("HS")]
    public class HighScoreCommand : Command
    {

        [Call]
        [Usage("Shows the people in the specified range. Defaults to 1-30")]
        private async Task ShowSheetAsync(int? from = null, int? to = null)
        {
            var sheet = await Context.TT2DataService.GetHighScores();

            if (from == null && to == null)
            {
                from = 0;
                to = 30;
            }
            else if (to == null)
                to = from;

            var start = from.Value;
            var end = to.Value;

            NumberExtensions.EnsureOrder(ref start, ref end);

            var places = Enumerable.Range(start, end - start + 1)
                                   .Select(i => sheet.Users.FirstOrDefault(u => u.Ranking == i))
                                   .Where(p => p != null)
                                   .ToList();

            if (places.Count == 0)
            {
                await ReplyAsync($"There were no users for the range {from} - {to}!");
                return;
            }

            var data = places.Select(p => new string[] { p.Ranking.ToString(), p.TotalRelics, p.UserName, p.ClanName }).ToArray();
            data = new string[][] { new string[] { "##", " Relics", " Username", " Clan" } }.Concat(data).ToArray();

            await ReplyAsync($"Here are the currently known users in rank {start} - {end}:\n```md\n{data.Tableify("[{0}]", "{0}  ")}```");

            return;
        }
    }
}
