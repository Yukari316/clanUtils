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

            #region 计算总平均值和单独boss平均值
            //各boss的平均伤害
            List<double> AtkAvgDmg = new List<double>();
            //各boss的出刀计数
            List<int> AtkCount = new List<int>();
            //Boss1
            AtkAvgDmg.Add(atkList.Where(atk =>
                                            atk.Attack != AttackType.Compensate     &&
                                            atk.Attack != AttackType.CompensateKill &&
                                            atk.Order  == 1)
                                 .Average(atk => atk.Damage));
            AtkCount.Add(atkList.Count(atk => atk.Attack != AttackType.Compensate     &&
                                              atk.Attack != AttackType.CompensateKill &&
                                              atk.Order  == 1));
            //Boss2
            AtkAvgDmg.Add(atkList.Where(atk =>
                                            atk.Attack != AttackType.Compensate     &&
                                            atk.Attack != AttackType.CompensateKill &&
                                            atk.Order  == 2)
                                 .Average(atk => atk.Damage));
            AtkCount.Add(atkList.Count(atk => atk.Attack != AttackType.Compensate     &&
                                              atk.Attack != AttackType.CompensateKill &&
                                              atk.Order  == 2));
            //Boss3
            AtkAvgDmg.Add(atkList.Where(atk =>
                                            atk.Attack != AttackType.Compensate     &&
                                            atk.Attack != AttackType.CompensateKill &&
                                            atk.Order  == 3)
                                 .Average(atk => atk.Damage));
            AtkCount.Add(atkList.Count(atk => atk.Attack != AttackType.Compensate     &&
                                              atk.Attack != AttackType.CompensateKill &&
                                              atk.Order  == 3));
            //Boss4
            AtkAvgDmg.Add(atkList.Where(atk =>
                                            atk.Attack != AttackType.Compensate     &&
                                            atk.Attack != AttackType.CompensateKill &&
                                            atk.Order  == 4)
                                 .Average(atk => atk.Damage));
            AtkCount.Add(atkList.Count(atk => atk.Attack != AttackType.Compensate     &&
                                              atk.Attack != AttackType.CompensateKill &&
                                              atk.Order  == 4));
            //Boss5
            AtkAvgDmg.Add(atkList.Where(atk =>
                                            atk.Attack != AttackType.Compensate     &&
                                            atk.Attack != AttackType.CompensateKill &&
                                            atk.Order  == 5)
                                 .Average(atk => atk.Damage));
            AtkCount.Add(atkList.Count(atk => atk.Attack != AttackType.Compensate     &&
                                              atk.Attack != AttackType.CompensateKill &&
                                              atk.Order  == 5));
            #endregion
                                     
            //计算个人单独总和数据
            foreach (MemberInfo memberInfo in memList)
            {
                ConsoleLog.Info("伤害统计",$"正在统计 {memberInfo.Name} 的伤害");
                #region 统计每个boss的伤害数值
                List<BossDmg> bossDmgData = new List<BossDmg>();
                for (int i = 1; i < 6; i++)
                {
                    ;
                    BossDmg bossDmg = new BossDmg
                    {
                        Dmg = atkList.Where(atk => atk.Order == i && atk.Uid == memberInfo.Uid &&
                                                   (atk.Attack != AttackType.Compensate ||
                                                    atk.Attack != AttackType.CompensateKill))
                                     .Sum(dmg => dmg.Damage),
                        Count = atkList.Where(atk => atk.Uid == memberInfo.Uid  && 
                                                     (atk.Attack != AttackType.Compensate || 
                                                      atk.Attack != AttackType.CompensateKill))
                                       .Count(atk => atk.Order == i),
                    };
                    bossDmg.AvgDmg    = Math.Round((double) bossDmg.Dmg / bossDmg.Count);
                    //计算相对标准偏差
                    bossDmg.Deviation = bossDmg.Count == 0 ? 0 : Math.Round(Math.Sqrt(atkList
                                                                                    .Where(atk => atk.Uid == memberInfo.Uid &&
                                                                                        (atk.Attack != AttackType.Compensate ||
                                                                                            atk.Attack != AttackType.CompensateKill))
                                                                                    .Sum(atk => Math.Pow(atk.Damage - AtkAvgDmg[i - 1], 2))
                                                                              / (bossDmg.Count == 0 ? 1 : bossDmg.Count))
                                                                          / AtkAvgDmg[i - 1] * 100, 2) *
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
                atkDatas.Add(atkData);
                ConsoleLog.Info("伤害统计", $"得到  {atkData.Name}  的总伤害为  {atkData.TotalDmg}  ({atkData.TotalTimes})");
            }
            return atkDatas;
        }
    }
}
