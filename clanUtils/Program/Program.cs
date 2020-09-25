using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using clanUtils.DatabaseUtils;
using clanUtils.Res;
using clanUtils.Utils;

namespace clanUtils.Program
{
    static class Program
    {
        ///使用指令 【clanUtils [数据库路径] [工会所在群号]】创建新的总伤害统计表
        ///工会所在群号可以缺省
        static int Main(string[] args)
        {
            Console.WriteLine($"clanUtils会战出刀统计工具 by饼干\r\nVersion {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            //参数检查
            if (args.Length < 1 || !File.Exists(args[0]) || args.Length > 2) return -1;
            GenAtkTable(args);
            return 0;
        }

        static void GenAtkTable(string[] args)
        {
            //创建数据库实例
            DatabaseHelper suiseiDb = new DatabaseHelper(args[0]);
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
                    Console.WriteLine("请选择工会对应群号的编号：");
                    guilds.ForEach(guild => Console.WriteLine($"{guilds.IndexOf(guild)} - {guild.Gid} - {guild.GuildName}"));
                    int getIndex = -1;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    while (getIndex < 0 || getIndex > guilds.Count - 1)
                    {
                        if (int.TryParse(Console.ReadLine(), out getIndex)) //尝试转换输入
                        {
                            //检查输入数值合法性
                            if (getIndex < 0 || getIndex > guilds.Count - 1) Console.WriteLine("弟啊你选了啥啊,重新写个编号");
                        }
                        else 
                        {
                            Console.WriteLine("弟啊你这不是数字啊,重新写个编号");
                            getIndex = -1;
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.White;
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
                return;
            }
            //处理伤害数据
            ConsoleLog.Info("数据库","正在从数据库读取数据...");
            List<AtkData> atkDatas = DMGParse.ParseAtkDatas(suiseiDb.GetAllAtk(), suiseiDb.GetAllMember());
            //生成伤害统计表
            ConsoleLog.Info("Excel生成","正在写入数据到文件...");
            WorkBookParse.GenerateDmgWorkbook(atkDatas, suiseiDb.GuildInfo.GuildName);
            ConsoleLog.Info("Excel生成","伤害统计完成\r\n");
            Console.WriteLine("按下任意键退出程序");
            Console.ReadKey();
        }
    }
}