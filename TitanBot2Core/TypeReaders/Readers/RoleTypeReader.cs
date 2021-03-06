﻿using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders.Readers
{
    public class RoleTypeReader<T> : TypeReader
        where T : class, IRole
    {
        public override Task<TypeReaderResponse> Read(CmdContext context, string input)
        {
            ulong id;

            if (context.Guild != null)
            {
                var results = new Dictionary<ulong, TypeReaderValue>();
                var roles = context.Guild.Roles;

                //By Mention (1.0)
                if (MentionUtils.TryParseRole(input, out id))
                    AddResult(results, context.Guild.GetRole(id) as T, 1.00f);

                //By Id (0.9)
                if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    AddResult(results, context.Guild.GetRole(id) as T, 0.90f);

                //By Name (0.7-0.8)
                foreach (var role in roles.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, role as T, role.Name == input ? 0.80f : 0.70f);

                if (results.Count > 0)
                    return Task.FromResult(TypeReaderResponse.FromSuccess(results.Values));
            }
            return Task.FromResult(TypeReaderResponse.FromError("Role not found."));
        }

        private void AddResult(Dictionary<ulong, TypeReaderValue> results, T role, float score)
        {
            if (role != null && !results.ContainsKey(role.Id))
                results.Add(role.Id, new TypeReaderValue(role, score));
        }
    }
}
