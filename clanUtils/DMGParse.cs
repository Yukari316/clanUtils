using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clanUtils
{
    internal class DMGParse
    {
        public static void GetDMG(ref Member member,int bossID, SQLiteConnection SQLConnection,string tableName)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                cmd.CommandText = $"SELECT * FROM {tableName}" +
                                $" WHERE uid='{member.uid}'";
                if (bossID != 0) cmd.CommandText += $" AND boss='{bossID}'";

                //读取相应uid的伤害数据
                SQLiteDataReader memberDataReader = cmd.ExecuteReader();
                int times = 0;
                long currentDmg = 0,avgDMG;
                while (memberDataReader.Read())
                {
                    times++;
                    currentDmg += Convert.ToInt32(memberDataReader["dmg"]);
                }
                avgDMG = currentDmg / (times == 0 ? 1 : times);
                member.GetType().GetProperty($"boss{bossID}_times").SetValue(member, times);
                member.GetType().GetProperty($"boss{bossID}_dmg").SetValue(member, currentDmg);
                member.GetType().GetProperty($"boss{bossID}_avg_dmg").SetValue(member, avgDMG);
            }
        }

        public static void GetTotalDMG(ref Member member)
        {
            member.total_dmg =
                member.boss1_dmg +
                member.boss2_dmg +
                member.boss3_dmg +
                member.boss4_dmg +
                member.boss5_dmg;
            member.times =
                member.boss1_times +
                member.boss2_times +
                member.boss3_times +
                member.boss4_times +
                member.boss5_times;
            member.avg_dmg = member.total_dmg / (member.times == 0 ? 1 : member.times);
        }
    }
}
