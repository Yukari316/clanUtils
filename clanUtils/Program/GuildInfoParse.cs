using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using clanUtils.DatabaseUtils;
using clanUtils.Res;
using clanUtils.Utils;

namespace clanUtils.Program
{
    internal static class GuildInfoParse
    {
        public static void GetGuildInfo(ref DatabaseHelper suiseiDb,string[] args)
        {
            //检查群号合法性
            if (args.Length == 2)
            {
                if (!long.TryParse(args[1], out long gid))
                {
                    ConsoleLog.Error("参数错误","非法参数");
                    Thread.Sleep(5000);
                    return;
                }
                if (!suiseiDb.GuildExists(gid))
                {
                    ConsoleLog.Error("参数错误","公会不存在");
                    Thread.Sleep(5000);
                    return;
                }
                suiseiDb.GuildInfo = suiseiDb.GetGuildInfo(gid);
            }
            //没有输入群号则获取
            else
            {
                List<GuildInfoLite> guilds = suiseiDb.GetGuildInfos();
                if (guilds == null || guilds.Count == 0)
                {
                    ConsoleLog.Error("数据库错误","弟啊你这库里啥都没有啊（半恼）");
                    Thread.Sleep(5000);
                    return;
                }
                //数据库中只有一个公会信息
                if (guilds.Count == 1)
                {
                    suiseiDb.GuildInfo = guilds.First();
                    ConsoleLog.Info("公会信息检查",$"检测到单一公会,已自动选择公会 {suiseiDb.GuildInfo.GuildName}");
                }
                //有多个公会信息
                else
                {
                    ConsoleLog.Info("公会信息检查",$"检测到 {guilds.Count} 个公会信息");
                    ConsoleLog.Info("选择公会","请选择工会对应群号的编号：");
                    guilds.ForEach(guild => ConsoleLog.Info("公会",$"{guilds.IndexOf(guild)} - {guild.Gid} - {guild.GuildName}"));
                    int getIndex = -1;
                    while (getIndex < 0 || getIndex > guilds.Count - 1)
                    {
                        if (int.TryParse(Console.ReadLine(), out getIndex)) //尝试转换输入
                        {
                            //检查输入数值合法性
                            if (getIndex < 0 || getIndex > guilds.Count - 1) ConsoleLog.Error("非法编号"," 弟啊你选了啥啊,重新写个编号");
                        }
                        else 
                        {
                            ConsoleLog.Error("非法编号"," 弟啊你这不是数字啊,重新写个编号");
                            getIndex = -1;
                        }
                        Console.SetCursorPosition(0,Console.WindowHeight - 2);
                        Console.Write(new string(' ',Console.WindowWidth));
                        Console.SetCursorPosition(0,Console.WindowHeight - 2);
                    }
                    suiseiDb.GuildInfo      = guilds[getIndex];
                    ConsoleLog.Info("公会信息检查",$"已选择公会 {suiseiDb.GuildInfo.GuildName}");
                }
            }
            //检查公会是否启用会战统计
            if (!suiseiDb.GuildInfo.InBattle ||
                suiseiDb.AtkDataEmpty()      ||
                !suiseiDb.AtkTableExists())
            {
                ConsoleLog.Error("arg error","所选公会未开启会战统计");
                Thread.Sleep(5000);
            }
            ConsoleLog.statusConsole.WriteLine("选择公会:");
            ConsoleLog.statusConsole.WriteLine($"{suiseiDb.GuildInfo.GuildName}");
        }
    }
}
