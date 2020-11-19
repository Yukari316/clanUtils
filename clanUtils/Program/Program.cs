using System;
using System.Collections.Generic;
using System.IO;
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
            ConsoleLog.Info("",$"clanUtils会战出刀统计工具 by饼干 Version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            //参数检查
            if (args.Length < 1 || !File.Exists(args[0]) || args.Length > 2) return -1;
            GenAtkTable(args);
            return 0;
        }

        static void GenAtkTable(string[] args)
        {
            //创建数据库实例
            DatabaseHelper db = new DatabaseHelper(args[0]);
            //处理输入参数
            GuildInfoParse.GetGuildInfo(ref db,args);
            //处理伤害数据
            ConsoleLog.Info("数据库","正在从数据库读取数据...");
            List<AtkData> atkDatas = DMGParse.ParseAtkDatas(db.GetAllAtk(), db.GetAllMember());
            //生成伤害统计表
            ConsoleLog.Info("Excel生成","正在写入数据到文件...");
            WorkBookParse.GenerateDmgWorkbook(atkDatas, db.GuildInfo.GuildName);
            ConsoleLog.Info("Excel生成","伤害统计完成\r\n");
            ConsoleLog.Info("","按下任意键退出程序");
            Console.ReadKey();
        }
    }
}