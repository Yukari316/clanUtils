using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clanUtils
{
    internal class Member
    {
        public long uid { get; set; }
        public string name { get; set; }
        public long total_dmg { get; set; } //总伤害
        public int times { get; set; }       //出刀数量
        public long avg_dmg { get; set; }//平均伤害
        //各个boss的伤害及出刀次数
        public long boss1_dmg { get; set; }
        public int boss1_times { get; set; }
        public long boss1_avg_dmg { get; set; }
        public long boss2_dmg { get; set; }
        public int boss2_times { get; set; }
        public long boss2_avg_dmg { get; set; }
        public long boss3_dmg { get; set; }
        public int boss3_times { get; set; }
        public long boss3_avg_dmg { get; set; }
        public long boss4_dmg { get; set; }
        public int boss4_times { get; set; }
        public long boss4_avg_dmg { get; set; }
        public long boss5_dmg { get; set; }
        public int boss5_times { get; set; }
        public long boss5_avg_dmg { get; set; }
    }
}
