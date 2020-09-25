using System.Collections.Generic;
using System.Linq;
using clanUtils.Res;
using SqlSugar;

namespace clanUtils.DatabaseUtils
{
    internal class DatabaseHelper
    {
        #region 属性
        private SqlSugarClient DbClient  { set; get; }
        public  GuildInfoLite  GuildInfo { set; get; }
        #endregion

        #region 构造函数
        public DatabaseHelper(string DBPath)
        {
            DbClient = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString      = $"DATA SOURCE={DBPath}",
                DbType                = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType           = InitKeyType.Attribute
            });
        }
        #endregion

        #region 数据库方法
        public List<GuildInfoLite> GetGuildInfos()
        {
            List<GuildInfoLite> guildInfo = new List<GuildInfoLite>();
            DbClient.Queryable<GuildInfo>()
                    .Select(guild => new {guild.Gid, guild.GuildName, guild.InBattle})
                    .ToList()
                    .ForEach(info => guildInfo.Add(new GuildInfoLite
                    {
                        Gid       = info.Gid,
                        GuildName = info.GuildName,
                        InBattle  = info.InBattle
                    }));
            return guildInfo;
        }

        public GuildInfoLite GetGuildInfo(long gid)
        {
            var guildInfo = DbClient.Queryable<GuildInfo>()
                                    .Where(guild => guild.Gid == gid)
                                    .Select(guild => new {guild.Gid, guild.GuildName, guild.InBattle})
                                    .First();
            return new GuildInfoLite
            {
                Gid       = guildInfo.Gid,
                GuildName = guildInfo.GuildName,
                InBattle  = guildInfo.InBattle
            };
        }

        public bool GuildExists(long gid)
        {
            return DbClient.Queryable<GuildInfo>()
                           .Where(guild => guild.Gid == gid)
                           .Any();
        }

        public bool AtkDataEmpty()
        {
            return DbClient.Queryable<GuildBattle>()
                           .AS($"guildbattle_{GuildInfo.Gid}")
                           .Count() == 0;
        }

        public bool AtkTableExists()
        {
            return DbClient.DbMaintenance
                           .GetTableInfoList()
                           .Any(table => table.DbObjectType == DbObjectType.Table &&
                                         table.Name         == $"guildbattle_{GuildInfo.Gid}");
        }

        public List<GuildBattle> GetAllAtk()
        {
            return DbClient.Queryable<GuildBattle>()
                           .AS($"guildbattle_{GuildInfo.Gid}")
                           .ToList();
        }

        public List<MemberInfo> GetAllMember()
        {
            return DbClient.Queryable<MemberInfo>()
                           .Where(member => member.Gid == GuildInfo.Gid)
                           .ToList();
        }
        #endregion
    }
}
