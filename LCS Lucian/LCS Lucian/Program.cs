﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Spells = LCS_Lucian.LucianSpells;

namespace LCS_Lucian
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Game.Mode == GameMode.Running)
            {
                OnLoad(null);
            }
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Lucian")
            {
                return;
            }

            LucianMenu.Config =
                new Menu("LCS Series: Lucian", "LCS Series: Lucian", true).SetFontStyle(System.Drawing.FontStyle.Bold,
                    Color.Gold);
            {
                Spells.Init();
                LucianMenu.OrbwalkerInit();
                LucianMenu.MenuInit();
            }

            Game.PrintChat("<font color='#99FFFF'>LCS Series - Lucian loaded! </font><font color='#99FF00'> Be Rekkles ! Its Possible. Enjoy GODSPEED Spell + Passive Usage </font>");
            Game.PrintChat("<font color='##FFCC00'>LCS Series totally improved LCS player style.</font>");

            Game.OnUpdate += LucianOnUpdate;
            Obj_AI_Base.OnDoCast += LucianOnDoCast;
            Drawing.OnDraw += LucianOnDraw;
        }
        public static bool UltActive
        {
            get { return ObjectManager.Player.HasBuff("LucianR"); }
        }
        private static void ECast(Obj_AI_Hero enemy)
        {
            var range = Orbwalking.GetRealAutoAttackRange(enemy);
            var path = Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                Prediction.GetPrediction(enemy, 0.25f).UnitPosition.To2D(), LucianSpells.E.Range, range);

            if (path.Count() > 0)
            {
                var epos = path.MinOrDefault(x => x.Distance(Game.CursorPos));
                if (epos.To3D().UnderTurret(true) || epos.To3D().IsWall())
                {
                    return;
                }

                if (epos.To3D().CountEnemiesInRange(LucianSpells.E.Range - 100) > 0)
                {
                    return;
                }
                LucianSpells.E.Cast(epos);
            }
            if (path.Count() == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -LucianSpells.E.Range);
                if (epos.UnderTurret(true) || epos.IsWall())
                {
                    return;
                }

                // no intersection or target to close
                LucianSpells.E.Cast(ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -LucianSpells.E.Range));
            }
        }
        private static void LucianOnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Hero && args.Target.IsValid)
            {
                if (Helper.LEnabled("lucian.combo.start.e"))
                {
                    if (!LucianSpells.E.IsReady() && LucianSpells.Q.IsReady() && Helper.LEnabled("lucian.q.combo") &&
                    ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        LucianSpells.Q.CastOnUnit(((Obj_AI_Hero)args.Target));
                    }
                    
                    if (!LucianSpells.E.IsReady() && LucianSpells.W.IsReady() && Helper.LEnabled("lucian.w.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.W.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        if (Helper.LEnabled("lucian.disable.w.prediction"))
                        {
                            LucianSpells.W.Cast(((Obj_AI_Hero)args.Target).Position);
                        }
                        else
                        {
                            if (LucianSpells.W.GetPrediction(((Obj_AI_Hero)args.Target)).Hitchance >= HitChance.Medium)
                            {
                                LucianSpells.W.Cast(((Obj_AI_Hero)args.Target).Position);
                            }
                        }
                       
                    }
                    if (LucianSpells.E.IsReady() && Helper.LEnabled("lucian.e.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.Q2.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (LucianMenu.Config.Item("lucian.e.mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                ECast(((Obj_AI_Hero)args.Target));
                                break;
                            case 1:
                                LucianSpells.E.Cast(Game.CursorPos);
                                break;
                        }
                        
                    }
                }
                else
                {
                    if (LucianSpells.Q.IsReady() && Helper.LEnabled("lucian.q.combo") &&
                    ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        LucianSpells.Q.CastOnUnit(((Obj_AI_Hero)args.Target));
                    }
                    if (LucianSpells.W.IsReady() && Helper.LEnabled("lucian.w.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.W.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff")
                        && LucianSpells.W.GetPrediction(((Obj_AI_Hero)args.Target)).Hitchance >= HitChance.Medium)
                    {
                        LucianSpells.W.Cast(((Obj_AI_Hero)args.Target).Position);
                    }
                    if (LucianSpells.E.IsReady() && Helper.LEnabled("lucian.e.combo") &&
                        ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.Q2.Range &&
                        LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                    {
                        switch (LucianMenu.Config.Item("lucian.e.mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                ECast(((Obj_AI_Hero)args.Target));
                                break;
                            case 1:
                                LucianSpells.E.Cast(Game.CursorPos);
                                break;
                        }
                    }
                }
                
            }
            else if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion && args.Target.IsValid && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral
                && ObjectManager.Player.ManaPercent > Helper.LSlider("lucian.clear.mana"))
            {
                if (LucianSpells.Q.IsReady() && Helper.LEnabled("lucian.q.jungle") &&
                    ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.Q.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.Q.CastOnUnit(((Obj_AI_Minion)args.Target));
                }
                if (LucianSpells.W.IsReady() && Helper.LEnabled("lucian.w.jungle") &&
                    ObjectManager.Player.Distance(args.Target.Position) < LucianSpells.W.Range &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.W.Cast(((Obj_AI_Minion)args.Target).Position);
                }
                if (LucianSpells.E.IsReady() && Helper.LEnabled("lucian.e.jungle") &&
                   ((Obj_AI_Minion)args.Target).IsValidTarget(1000) &&
                    LucianMenu.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
                {
                    LucianSpells.E.Cast(Game.CursorPos);
                }

            }
        }
        private static void LucianOnUpdate(EventArgs args)
        {
            switch (LucianMenu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }

            if (LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                SemiManual();
            }

            if (UltActive && LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                LucianMenu.Orbwalker.SetAttack(false);
            }

            if (!UltActive || !LucianMenu.Config.Item("lucian.semi.manual.ult").GetValue<KeyBind>().Active)
            {
                LucianMenu.Orbwalker.SetAttack(true);
            }

            if (!UltActive && Helper.LEnabled("use.eq"))
            {
                if (Spells.E.IsReady() && 
                    ObjectManager.Player.CountEnemiesInRange(Helper.LSlider("eq.safety.range")) <= Helper.LSlider("eq.min.enemy.count.range"))
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.Q.Range + Spells.E.Range - 100 )))
                    {
                        var aadamage = ObjectManager.Player.GetAutoAttackDamage(enemy);
                        var dmg = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical,
                            Spells.Q.GetDamage(enemy));
                        var combodamage = aadamage + dmg;

                        if (enemy.Health < combodamage)
                        {
                            Spells.E.Cast(ObjectManager.Player.Position.Extend(enemy.Position, Spells.E.Range));
                        }
                    }
                    
                    if (Spells.Q.IsReady() && ObjectManager.Player.CountEnemiesInRange(Helper.LSlider("eq.safety.range")) <= Helper.LSlider("eq.min.enemy.count.range"))
                    {
                        foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.Q.Range)))
                        {
                            var dmg = ObjectManager.Player.CalcDamage(enemy, Damage.DamageType.Physical,
                                Spells.Q.GetDamage(enemy));
                            var aadamage = ObjectManager.Player.GetAutoAttackDamage(enemy);

                            var combodamage = aadamage + dmg;

                            if (enemy.Health < combodamage)
                            {
                                Spells.Q.CastOnUnit(enemy);
                            }
                        }
                    }
                }
            }
            if (!UltActive && Helper.LEnabled("lucian.q.ks") && LucianSpells.Q.IsReady())
            {
                ExtendedQKillSteal();
            }
            
            if (!UltActive  && Helper.LEnabled("lucian.w.ks") && LucianSpells.W.IsReady())
            {
                KillstealW();
            }


        }
        private static void SemiManual()
        {
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(LucianSpells.R.Range) &&
                LucianSpells.R.GetPrediction(x).CollisionObjects.Count < 2))
            {
                LucianSpells.R.Cast(enemy);
            }

        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Helper.LSlider("lucian.harass.mana"))
            {
                return;
            }
            if (LucianSpells.Q.IsReady() || LucianSpells.Q2.IsReady() && Helper.LEnabled("lucian.q.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                HarassQCast();
            }
            if (LucianSpells.W.IsReady() && Helper.LEnabled("lucian.w.harass") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(LucianSpells.W.Range) && LucianSpells.W.GetPrediction(x).Hitchance >= HitChance.Medium))
                {
                    LucianSpells.W.Cast(enemy);
                }
            }
        }
        private static void HarassQCast()
        {
            switch (LucianMenu.Config.Item("lucian.q.type").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(LucianSpells.Q.Range));
                    var target = ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsValidTarget(LucianSpells.Q2.Range)).FirstOrDefault(x => LucianMenu.Config.Item("lucian.white" + x.ChampionName).GetValue<bool>());
                    if (target.Distance(ObjectManager.Player.Position) > LucianSpells.Q.Range && target.CountEnemiesInRange(LucianSpells.Q2.Range) > 0)
                    {
                        foreach (var minion in minions)
                        {
                            if (LucianSpells.Q2.WillHit(target, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, LucianSpells.Q2.Range), 0, HitChance.VeryHigh))
                            {
                                LucianSpells.Q2.CastOnUnit(minion);
                            }
                        }
                    }
                    break;
                case 1:
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(LucianSpells.Q.Range)))
                    {
                        LucianSpells.Q.CastOnUnit(enemy);
                    }
                    break;
            }
        }
        private static void ExtendedQKillSteal()
        {
            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(o => o.IsValidTarget(LucianSpells.Q.Range));
            var target = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(LucianSpells.Q2.Range));
            
            if (target != null && (target.Distance(ObjectManager.Player.Position) > LucianSpells.Q.Range &&
                                   target.Distance(ObjectManager.Player.Position) < LucianSpells.Q2.Range && 
                                   target.CountEnemiesInRange(LucianSpells.Q2.Range) >= 1 && target.Health < LucianSpells.Q.GetDamage(target) && !target.IsDead))
            {
                foreach (var minion in minions)
                {
                    if (LucianSpells.Q2.WillHit(target, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, LucianSpells.Q2.Range),0,HitChance.VeryHigh))
                    {
                        LucianSpells.Q2.CastOnUnit(minion);
                    }
                }
            }
        }
        private static void KillstealW()
        {
            var target = HeroManager.Enemies.Where(x => x.IsValidTarget(LucianSpells.W.Range)).
                FirstOrDefault(x=> x.Health < LucianSpells.W.GetDamage(x));

            var pred = LucianSpells.W.GetPrediction(target);

            if (target != null && pred.Hitchance >= HitChance.High)
            {
                LucianSpells.W.Cast(pred.CastPosition);
            }
        }
        private static void Clear()
        {
            if (ObjectManager.Player.ManaPercent < Helper.LSlider("lucian.clear.mana"))
            {
                return;
            }
            if (LucianSpells.Q.IsReady() && Helper.LEnabled("lucian.q.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.ServerPosition, LucianSpells.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly))
                {
                    var prediction = Prediction.GetPrediction(minion, LucianSpells.Q.Delay,
                        ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.CastRadius);

                    var collision = LucianSpells.Q.GetCollision(ObjectManager.Player.Position.To2D(),
                        new List<Vector2> { prediction.UnitPosition.To2D() });

                    foreach (var cs in collision)
                    {
                        if (collision.Count >= Helper.LSlider("lucian.q.minion.hit.count"))
                        {
                            if (collision.Last().Distance(ObjectManager.Player) -
                                collision[0].Distance(ObjectManager.Player) <= 600
                                && collision[0].Distance(ObjectManager.Player) <= 500)
                            {
                                LucianSpells.Q.Cast(cs);
                            }
                        }
                    }

                }
            }
            if (LucianSpells.W.IsReady() && Helper.LEnabled("lucian.w.clear") && ObjectManager.Player.Buffs.Any(buff => buff.Name != "lucianpassivebuff"))
            {
                if (LucianSpells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, LucianSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).MinionsHit >= Helper.LSlider("lucian.w.minion.hit.count"))
                {
                    LucianSpells.W.Cast(LucianSpells.W.GetCircularFarmLocation(MinionManager.GetMinions(ObjectManager.Player.Position, LucianSpells.Q.Range, MinionTypes.All, MinionTeam.NotAlly)).Position);
                }
            }
        }
        private static void LucianOnDraw(EventArgs args)
        {
            LucianDrawing.Init();
        }
    }
}
