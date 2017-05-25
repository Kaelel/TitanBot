﻿using TitanBot2.Services.Database.Models;

namespace TitanBot2.Services.Database.Extensions
{
    public class RegistrationExtensions : DatabaseExtension<Registration>
    {
        public RegistrationExtensions(BotDatabase db) : base(db)
        {
        }
    }
}
