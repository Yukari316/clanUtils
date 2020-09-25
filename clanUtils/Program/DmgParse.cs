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
            foreach (MemberInfo memberInfo in memList)
            {
                ConsoleLog.Info("伤害统计",$"正在统计 {memberInfo.Name} 的伤害");
                #region 统计每个boss的伤害数值
                List<BossDmg> bossDmgData = new List<BossDmg>();
                for (int i = 1; i < 6; i++)
                {
                    BossDmg bossDmg = new BossDmg
                    {
                        Dmg = atkList.Where(atk => atk.Order == i &&atk.Uid ==memberInfo.Uid)
                                     .Sum(dmg=>dmg.Damage),
                        Times = atkList.Where(member => member.Uid == memberInfo.Uid)
                                       .Count(atk => atk.Order     == i)
                    };
                    bossDmg.AvgDmg = (double)bossDmg.Dmg / bossDmg.Times;
                    bossDmgData.Add(bossDmg);
                }
                #endregion
                //计算伤害总和数据
                AtkData atkData = new AtkData
                {
                    Uid = memberInfo.Uid,
                    Name = memberInfo.Name,
                    TotalDmg = bossDmgData.Select(dmg => dmg.Dmg)
                                          .Sum(),
                    TotalTimes = bossDmgData.Select(times=>times.Times)
                                            .Sum(),
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
