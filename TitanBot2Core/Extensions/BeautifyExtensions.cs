﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using TitanBot2.Models.Enums;

namespace TitanBot2.Extensions
{
    public static class BeautifyExtensions
    {
        private static readonly string _alphabet = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string[] _postfixes = new string[] { "", "K", "M", "B", "T" };
        private static readonly string _infinity = "∞";

        public static string Beautify(this double value)
        {
            if (double.IsInfinity(value))
                return _infinity;
            if (double.IsNaN(value))
                return "NaN";
            if (value == 0)
                return "0";
            var isNegative = value < 0;
            value = Math.Abs(value);
            var magnitude = (int)Math.Floor(Math.Log10(value)) / 3;
            string postfix;

            if (magnitude > _postfixes.Length - 1)
                postfix = _alphabet[(magnitude - (_postfixes.Length)) / 26].ToString() +
                          _alphabet[(magnitude - (_postfixes.Length)) % 26].ToString();
            else
                postfix = _postfixes[magnitude];

            return string.Format($"{(isNegative ? "-" : "")}{{0:0.##}}" + postfix, value / (Math.Pow(10, magnitude * 3)));
        }

        public static string Beautify(this int value)
        {
            if (value == int.MaxValue || value == int.MinValue)
                return _infinity;
            return string.Format("{0:#,##0}", value);
        }

        public static string Beautify(this DateTime date)
        {
            return date.ToString();
        }

        public static string Beautify(this TimeSpan timespan)
        {
            var returnString = timespan > new TimeSpan() ? "" : "-";
            if (timespan.Days != 0)
            {
                if (timespan.Days == 1)
                    returnString += $"{timespan.Days} day, ";
                else
                    returnString += $"{timespan.Days} days, ";
            }
            returnString += $"{Math.Abs(timespan.Hours).ToString().PadLeft(2, '0')}:";
            returnString += $"{Math.Abs(timespan.Minutes).ToString().PadLeft(2, '0')}:";
            returnString += $"{Math.Abs(timespan.Seconds).ToString().PadLeft(2, '0')}";

            return returnString;
        }

        public static string Beautify(this HelperType helperType)
        {
            switch (helperType)
            {
                case HelperType.Melee:
                    return "Melee";
                case HelperType.Ranged:
                    return "Ranged";
                case HelperType.Spell:
                    return "Spell";
                default:
                    return helperType.ToString();
            }
        }

        public static string Beautify(this BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.AllDamage:
                    return "All Damage";
                case BonusType.AllHelperDamage:
                    return "All Hero Damage";
                case BonusType.ArmorBoost:
                    return "Armor Equipment Bonus";
                case BonusType.ArtifactDamage:
                    return "Artifact Damage";
                case BonusType.BurstDamageSkillAmount:
                    return "Heavenly Strike Effect";
                case BonusType.BurstDamageSkillMana:
                    return "Heavenly Strike Cost";
                case BonusType.ChestAmount:
                    return "Chesterson Amount";
                case BonusType.ChestChance:
                    return "Chesterson Chance";
                case BonusType.CritBoostSkillDuration:
                    return "Critical Strike Duration";
                case BonusType.CritBoostSkillMana:
                    return "Critical Strike Cost";
                case BonusType.CritChance:
                    return "Critical Chance";
                case BonusType.DoubleFairyChance:
                    return "Double Fairy Chance";
                case BonusType.GoldAll:
                    return "All Gold";
                case BonusType.GoldBoss:
                    return "Boss Gold";
                case BonusType.GoldMonster:
                    return "Titan Gold";
                case BonusType.Goldx10Chance:
                    return "x10 Gold Chance";
                case BonusType.HandOfMidasSkillAmount:
                    return "Hand of Midas Effect";
                case BonusType.HandOfMidasSkillDuration:
                    return "Hand of Midas Duration";
                case BonusType.HandOfMidasSkillMana:
                    return "Hand of Midas Cost";
                case BonusType.HelmetBoost:
                    return "Helmet Equipment Bonus";
                case BonusType.HelperBoostSkillAmount:
                    return "War Cry Effect";
                case BonusType.HelperBoostSkillDuration:
                    return "War Cry Duration";
                case BonusType.HelperBoostSkillMana:
                    return "War Cry Cost";
                case BonusType.HelperUpgradeCost:
                    return "Hero Cost";
                case BonusType.HSArtifactDamage:
                    return "All Artifact Damage";
                case BonusType.MeleeHelperDamage:
                    return "Melee Hero Damage";
                case BonusType.PetDamageMult:
                    return "Pet Damage";
                case BonusType.PrestigeRelic:
                    return "Prestige Relics";
                case BonusType.RangedHelperDamage:
                    return "Ranged Hero Damage";
                case BonusType.ShadowCloneSkillAmount:
                    return "Shadow Clone Effect";
                case BonusType.ShadowCloneSkillDuration:
                    return "Shadow Clone Duration";
                case BonusType.ShadowCloneSkillMana:
                    return "Shadow Clone Cost";
                case BonusType.SlashBoost:
                    return "Slash Equipment Bonus";
                case BonusType.SpellHelperDamage:
                    return "Spell Hero Damage";
                case BonusType.SwordBoost:
                    return "Weapon Equipment Bonus";
                case BonusType.TapBoostSkillAmount:
                    return "Fire Sword Effect";
                case BonusType.TapBoostSkillDuration:
                    return "Fire Sword Duration";
                case BonusType.TapBoostSkillMana:
                    return "Fire Sword Cost";
                case BonusType.TapDamage:
                    return "Tap Damage";
                case BonusType.SplashDamage:
                    return "Splash Damage";
                case BonusType.ManaRegen:
                    return "Mana Regen";
                case BonusType.CritDamage:
                    return "Crit Damage";
                case BonusType.ManaPoolCap:
                    return "Mana Pool Cap";
                case BonusType.TapDamageFromHelpers:
                    return "% Tap Damage From Heroes";
                case BonusType.MonsterHP:
                    return "Titan HP";
                default:
                    return bonusType.ToString();
            }
        }

        public static string Beautify(this EquipmentClass equipmentType)
        {
            switch (equipmentType)
            {
                case EquipmentClass.Aura:
                    return "Aura";
                case EquipmentClass.Hat:
                    return "Hat";
                case EquipmentClass.None:
                    return "None";
                case EquipmentClass.Slash:
                    return "Slash";
                case EquipmentClass.Suit:
                    return "Suit";
                case EquipmentClass.Weapon:
                    return "Weapon";
                default:
                    return equipmentType.ToString();
            }
        }

        public static string FormatValue(this BonusType bonusType, double value)
        {
            value = Math.Round(value, 5);
            switch (bonusType)
            {
                case BonusType.CritBoostSkillDuration:
                case BonusType.HandOfMidasSkillDuration:
                case BonusType.HelperBoostSkillDuration:
                case BonusType.ShadowCloneSkillDuration:
                case BonusType.TapBoostSkillDuration:
                    return $"+{(int)value}s";
                case BonusType.BurstDamageSkillMana:
                case BonusType.CritBoostSkillMana:
                case BonusType.HandOfMidasSkillMana:
                case BonusType.HelperBoostSkillMana:
                case BonusType.ShadowCloneSkillMana:
                case BonusType.TapBoostSkillMana:
                    return $"-{(int)value} mana";
                case BonusType.HelperUpgradeCost:
                    return $"-{(int)(value * 100)}%";
                case BonusType.DoubleFairyChance:
                case BonusType.CritChance:
                case BonusType.Goldx10Chance:
                case BonusType.ChestChance:
                    return string.Format("{0:0.##}%", value * 100);
                case BonusType.HSArtifactDamage:
                    return $"x{1 + value}";
                case BonusType.MeleeHelperDamage:
                case BonusType.SpellHelperDamage:
                case BonusType.RangedHelperDamage:
                case BonusType.AllHelperDamage:
                case BonusType.CritDamage:
                case BonusType.PetDamageMult:
                case BonusType.MonsterHP:
                case BonusType.TapDamage:
                case BonusType.AllDamage:
                case BonusType.GoldAll:
                case BonusType.ChestAmount:
                case BonusType.GoldBoss:
                case BonusType.GoldMonster:
                case BonusType.HelperBoostSkillAmount:
                case BonusType.ShadowCloneSkillAmount:
                case BonusType.HandOfMidasSkillAmount:
                case BonusType.TapBoostSkillAmount:
                case BonusType.BurstDamageSkillAmount:
                case BonusType.PrestigeRelic:
                case BonusType.HelmetBoost:
                case BonusType.SwordBoost:
                case BonusType.SlashBoost:
                case BonusType.ArmorBoost:
                case BonusType.AuraBoost:
                    return $"x{value+1}";
                case BonusType.SplashDamage:
                case BonusType.ManaRegen:
                case BonusType.ManaPoolCap:
                    return $"+{value}";
                case BonusType.None:
                    return $"-";
                default:
                    return $"{(int)(value * 100)}%";

            }
        }

        public static bool TryUnbeautify(this string s, out double result)
        {
            if (double.TryParse(s, out result))
                return true;
            double modifier = 0;
            if (Regex.IsMatch(s, $@"\d *[{_alphabet}]{{2}}$", RegexOptions.IgnoreCase))
            {
                if (!double.TryParse(s.Substring(0, s.Length - 2), out result))
                    return false;
                modifier = _alphabet.IndexOf(s[s.Length - 2]) * 26 +
                           _alphabet.IndexOf(s[s.Length - 1]) + _postfixes.Length;
                modifier = Math.Pow(10, modifier * 3);
            }
            else if (Regex.IsMatch(s, $@"\d *[{string.Join("", _postfixes)}]$", RegexOptions.IgnoreCase))
            {
                if (!double.TryParse(s.Substring(0, s.Length - 1), out result))
                    return false;
                modifier = _postfixes.ToLower().ToList().IndexOf(s.Substring(s.Length - 1)) * 3;
                modifier = Math.Pow(10, modifier);
            }
            else
                return false;

            result = result * modifier;
            return true;
        }

        public static bool TryUnbeautify(this string s, out int result)
            => int.TryParse(s.Replace(",", ""), out result);
    }
}
