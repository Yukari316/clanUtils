using System.Collections.Generic;
using SqlSugar;
// ReSharper disable ClassNeverInstantiated.Global

namespace clanUtils.Res
{
    #region 出刀统计表定义

    internal class AtkData
    {
        public long          Uid          { get; set; }
        public string        Name         { get; set; }
        public long          TotalDmg     { get; set; }
        public double        TotalAvgDmg  { get; set; }
        public int           TotalTimes   { get; set; }
        public double        Deviation    { get; set; }
        public List<BossDmg> BossDmgInfos { get; set; }
    }

    internal class BossDmg
    {
        public long   Dmg       { get; set; }
        public double AvgDmg    { get; set; }
        public int    Count     { get; set; }
        public double Deviation { get; set; }
    }

    internal class GuildInfoLite
    {
        public long   Gid       { get; set; }
        public string GuildName { get; set; }
        public bool   InBattle  { get; set; }
    }
    #endregion

    #region 出刀记录表定义
    [SugarTable("guildbattle")]
    internal class GuildBattle
    {
        /// <summary>
        /// 记录编号[自增]
        /// </summary>
        [SugarColumn(ColumnName = "aid" , ColumnDataType = "INTEGER",IsIdentity = true,IsPrimaryKey = true)]
        public int Aid { get; set; }
        /// <summary>
        /// 用户QQ号
        /// </summary>
        [SugarColumn(ColumnName = "uid",ColumnDataType = "INTEGER")]
        public long Uid { get; set; }
        /// <summary>
        /// 记录产生时间
        /// </summary>
        [SugarColumn(ColumnName = "time",ColumnDataType = "INTEGER")]
        public long Time { get; set; }
        /// <summary>
        /// 周目数
        /// </summary>
        [SugarColumn(ColumnName = "round",ColumnDataType = "INTEGER")]
        public int Round { get; set; }
        /// <summary>
        /// boss的序号
        /// </summary>
        [SugarColumn(ColumnName = "order_num",ColumnDataType = "INTEGER")]
        public int Order { get; set; }
        /// <summary>
        /// 伤害数值
        /// </summary>
        [SugarColumn(ColumnName = "dmg",ColumnDataType = "INTEGER")]
        public long Damage { get; set; }
        /// <summary>
        /// 出刀类型标记
        /// </summary>
        [SugarColumn(ColumnName = "flag",ColumnDataType = "INTEGER")]
        public AttackType Attack { get; set; }
    }
    #endregion

    #region 成员表定义
    [SugarTable("member")]
    internal class MemberInfo
    {
        /// <summary>
        /// 用户所在群号，同时也是公会标识
        /// </summary>
        [SugarColumn(ColumnName = "gid",ColumnDataType = "INTEGER",IsPrimaryKey = true)]
        public long Gid { get; set; }
        /// <summary>
        /// 用户的QQ号
        /// </summary>
        [SugarColumn(ColumnName = "uid",ColumnDataType = "INTEGER",IsPrimaryKey = true)]
        public long Uid { get; set; }
        /// <summary>
        /// 成员名
        /// </summary>
        [SugarColumn(ColumnName = "name",ColumnDataType = "VARCHAR")]
        public string Name { get; set; }
        /// <summary>
        /// 用户状态修改时间
        /// </summary>
        [SugarColumn(ColumnName = "time",ColumnDataType = "INTEGER")]
        public long Time { get; set; }
        /// <summary>
        /// 用户状态标志
        /// </summary>
        [SugarColumn(ColumnName = "flag",ColumnDataType = "INTEGER")]
        public FlagType Flag { get; set; }
        /// <summary>
        /// 状态描述（可空，需按照文档进行修改）
        /// </summary>
        [SugarColumn(ColumnName = "info",ColumnDataType = "VARCHAR",IsNullable = true)]
        public string Info { get; set; }
        /// <summary>
        /// 当日SL标记,使用时间戳存储产生时间
        /// </summary>
        [SugarColumn(ColumnName = "sl",ColumnDataType = "INTEGER")]
        public long SL { get; set; }
    }
    #endregion

    #region 公会表定义
    [SugarTable("guild")]
    internal class GuildInfo
    {
        /// <summary>
        /// 公会所属群号
        /// </summary>
        [SugarColumn(ColumnName = "gid",ColumnDataType = "INTEGER",IsPrimaryKey = true)]
        public long Gid { get; set; }

        /// <summary>
        /// 公会名
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDataType = "VARCHAR")]
        public string GuildName { get; set; }

        /// <summary>
        /// 公会所在区服
        /// </summary>
        [SugarColumn(ColumnName = "server", ColumnDataType = "INTEGER")]
        public Server ServerId { get; set; }

        /// <summary>
        /// 当前boss的血量
        /// </summary>
        [SugarColumn(ColumnName = "hp",ColumnDataType = "INTEGER")]
        public long HP { get; set; }

        /// <summary>
        /// 当前boss的总血量
        /// </summary>
        [SugarColumn(ColumnName = "total_hp",ColumnDataType = "INTEGER")]
        public long TotalHP { get; set; }

        /// <summary>
        /// 当前公会所在周目
        /// </summary>
        [SugarColumn(ColumnName = "round",ColumnDataType = "INTEGER")]
        public int Round { get; set; }

        /// <summary>
        /// 当前所在boss序号
        /// </summary>
        [SugarColumn(ColumnName = "order_num",ColumnDataType = "INTEGER")]
        public int Order { get; set; }

        /// <summary>
        /// 当前boss阶段
        /// </summary>
        [SugarColumn(ColumnName = "boss_phase",ColumnDataType = "INTEGER")]
        public int BossPhase { get; set; }

        /// <summary>
        /// 公会是否在会战
        /// </summary>
        [SugarColumn(ColumnName = "in_battle",ColumnDataType = "INTEGER")]
        public bool InBattle { get; set; }
    }
    #endregion
}
