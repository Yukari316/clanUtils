using System;
using System.Collections.Generic;
using System.Linq;
using clanUtils.Res;
using clanUtils.Utils;

namespace clanUtils.Program
{
    internal static class DMGParse
    {
        public static List<AtkData> ParseAtkDatas(List<GuildBattle> atkList, List<MemberInfo> memList)
        {
            List<AtkData> atkDatas = new List<AtkData>();
            if (!atkList.Any(atk => atk.Attack != AttackType.Compensate || atk.Attack != AttackType.CompensateKill))
                return new List<AtkData>();

            List<GuildBattle> atkListParsed = new();
            ConsoleLog.Info("伤害统计","正在处理出刀数据");
            //合并补时刀与尾刀伤害
            foreach (var memberInfo in memList)
            {
                List<GuildBattle> memAtks = atkList.Where(member => member.Uid == memberInfo.Uid)
                                                   .ToList();
                //合并补时刀与尾刀
                for (int i = 0; i < memAtks.Count; i++)
                {
                    var atk = memAtks[i];
                    if(atk.Attack != AttackType.Compensate && atk.Attack != AttackType.CompensateKill) continue;
                    var atkIndex = memAtks.IndexOf(atk);
                    if(atkIndex == -1) continue;
                    memAtks[atkIndex - 1].Damage += memAtks[atkIndex].Damage;
                    memAtks.RemoveAt(atkIndex);
                    //回退遍历检查
                    i--;
                }
                atkListParsed.AddRange(memAtks);
            }

            #region 计算总平均值和单独boss平均值
            //总平均值
            double TotalAvgDmg = atkListParsed.Sum(atk => atk.Damage) /
                                 (double) atkListParsed.Count(atk => atk.Attack != AttackType.Compensate &&
                                                                     atk.Attack != AttackType.CompensateKill);
            //总出刀计数
            double TotalAtkCount = atkListParsed.Count(atk => atk.Attack != AttackType.Compensate &&
                                                              atk.Attack != AttackType.CompensateKill);
            //各boss的平均伤害
            List<double> AtkAvgDmg = new List<double>();
            //Boss1
            AtkAvgDmg.Add(atkListParsed.Where(atk =>
                                                  atk.Order == 1)
                                       .Sum(atk => atk.Damage) /
                          (double) atkListParsed.Count(atk => atk.Order  == 1 && atk.Attack != AttackType.Compensate &&
                                                              atk.Attack != AttackType.CompensateKill)
                         );
            //Boss2
            AtkAvgDmg.Add(atkListParsed.Where(atk =>
                                                  atk.Order == 2)
                                       .Sum(atk => atk.Damage) /
                          (double) atkListParsed.Count(atk => atk.Order  == 2 && atk.Attack != AttackType.Compensate &&
                                                              atk.Attack != AttackType.CompensateKill));
            //Boss3
            AtkAvgDmg.Add(atkListParsed.Where(atk =>
                                                  atk.Order  == 3)
                                       .Sum(atk => atk.Damage) /
                          (double) atkListParsed.Count(atk => atk.Order  == 3 && atk.Attack != AttackType.Compensate &&
                                                              atk.Attack != AttackType.CompensateKill));
            //Boss4
            AtkAvgDmg.Add(atkListParsed.Where(atk =>
                                                  atk.Order  == 4)
                                       .Sum(atk => atk.Damage) /
                          (double) atkListParsed.Count(atk => atk.Order  == 4 && atk.Attack != AttackType.Compensate &&
                                                              atk.Attack != AttackType.CompensateKill));
            //Boss5
            AtkAvgDmg.Add(atkListParsed.Where(atk =>
                                                  atk.Order  == 5)
                                       .Sum(atk => atk.Damage) /
                          (double) atkListParsed.Count(atk => atk.Order  == 5 && atk.Attack != AttackType.Compensate &&
                                                              atk.Attack != AttackType.CompensateKill));
            #endregion

            //计算个人单独总和数据
            foreach (MemberInfo memberInfo in memList)
            {
                ConsoleLog.Info("伤害统计",$"正在统计 {memberInfo.Name} 的伤害");

                #region 统计每个boss的伤害数值
                List<BossDmg> bossDmgData = new();
                for (int i = 1; i < 6; i++)
                {
                    //计算总量和数量
                    BossDmg bossDmg = new BossDmg
                    {
                        Dmg = atkListParsed.Where(atk => atk.Order == i &&
                                                         atk.Uid == memberInfo.Uid)
                                           .Sum(dmg => dmg.Damage),
                        Count = atkListParsed.Count(atk => atk.Order == i &&
                                                           atk.Uid   == memberInfo.Uid)
                    };
                    //计算平均值
                    bossDmg.AvgDmg = Math.Round((double) bossDmg.Dmg / bossDmg.Count);
                    //计算总体相对标准偏差
                    bossDmg.Deviation = bossDmg.Count == 0
                        ? 0
                        : Math.Round(Math.Sqrt(atkListParsed
                                               //当前boss的通常刀和尾刀
                                               .Where(atk => atk.Order == i &&
                                                             atk.Uid   == memberInfo.Uid)
                                               .Sum(atk => Math.Pow(atk.Damage - AtkAvgDmg[i - 1], 2))
                                             / bossDmg.Count) / AtkAvgDmg[i - 1] * 100, 2) *
                          (bossDmg.AvgDmg > AtkAvgDmg[i - 1] ? 1 : -1);
                    bossDmgData.Add(bossDmg);
                }
                #endregion
                //计算伤害总和数据
                AtkData atkData = new AtkData
                {
                    Uid          = memberInfo.Uid,
                    Name         = memberInfo.Name,
                    TotalDmg     = bossDmgData.Sum(dmg => dmg.Dmg),
                    TotalTimes   = bossDmgData.Sum(times=>times.Count),
                    BossDmgInfos = bossDmgData
                };
                atkData.TotalAvgDmg = (double) atkData.TotalDmg / atkData.TotalTimes;
                //计算总体相对标准偏差
                atkData.Deviation = Math.Round(Math.Sqrt(atkListParsed
                                                                 .Where(atk => atk.Uid == memberInfo.Uid)
                                                                 .Sum(atk => Math.Pow(atk.Damage - TotalAvgDmg, 2))
                                                               / TotalAtkCount) / TotalAvgDmg * 100, 2) *
                                    (atkData.TotalAvgDmg > TotalAvgDmg ? 1 : -1);
                atkDatas.Add(atkData);
                ConsoleLog.Info("伤害统计", $"得到  {atkData.Name}  的总伤害为  {atkData.TotalDmg}  ({atkData.TotalTimes})");
            }
            return atkDatas;
        }
    }
}
