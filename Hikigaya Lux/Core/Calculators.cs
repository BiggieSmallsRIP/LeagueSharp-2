﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Hikigaya_Lux.Core
{
    class Calculators
    {
        private static readonly double[] QDamage = { 60, 110, 160, 210, 260 };
        private static readonly double[] EDamage = { 60, 105, 150, 195, 240 };
        private static readonly double[] RDamage = { 300, 400, 500 };
        public static readonly Obj_AI_Hero Lux = ObjectManager.Player;

        public static float Q(Obj_AI_Base enemy)
        {
            switch (SelectCalculatorStyle())
            {
                case 1:
                    return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, QDamage[Spells.Q.Level - 1] + 0.7 * Lux.AbilityPower());
                case 2:
                    return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, Spells.Q.GetDamage(enemy));
                    
            }
            return 0.0f;
        }
        public static float E(Obj_AI_Base enemy)
        {
            switch (SelectCalculatorStyle())
            {
                case 1:
                    return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, EDamage[Spells.E.Level - 1] + 0.6 * Lux.AbilityPower());
                case 2:
                    return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, Spells.E.GetDamage(enemy));
            }
            return 0.0f;
            
        }
        public static float R(Obj_AI_Base enemy)
        {
            switch (SelectCalculatorStyle())
            {
                case 1:
                    return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, RDamage[Spells.R.Level - 1] + 0.75 * Lux.AbilityPower());
                case 2:
                    return (float)ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Magical, Spells.R.GetDamage(enemy));
            }
            return 0.0f;

            
        }

        public static byte SelectCalculatorStyle()
        {
            switch (LuxMenu.Config.Item("calculator").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
            }
            return 0;
        }
    }
}
