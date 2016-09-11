﻿using hYasuo.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

namespace hYasuo.Logics
{
    internal static class YasuoWW
    {
        public static void YasuoWindWallProtector(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy && sender.Type != GameObjectType.obj_AI_Hero && args.SData.IsAutoAttack())
            {
                return;
            }

            if (Spells.W.IsReady() && Menus.Config.Item("w.protect." + args.SData.Name).GetValue<bool>() && 
                Menus.Config.Item("w.protect."+args.SData.Name) != null && 
                args.End.Distance(ObjectManager.Player.Position) < 100 && sender is Obj_AI_Hero)
            {
                Spells.W.Cast(args.Start);
            }
        }

        public static void YasuoTargettedProtector(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy && sender.Type != GameObjectType.obj_AI_Hero && args.SData.IsAutoAttack()
                && args.SData.TargettingType != SpellDataTargetType.Unit)
            {
                return;
            }

            if (Spells.Q1.IsReady() && Spells.Q.Empowered() && Utilities.Enabled("q2.protect.targetted."+args.SData.Name) &&
                Menus.Config.Item("q2.protect.targetted." + args.SData.Name) != null && sender is Obj_AI_Hero)
            {
                Spells.Q1.Cast(args.End);
            }
        }
    }
}
