using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clanUtils
{
    class Program
    {
        ///使用指令 【clanUtils [数据库路径] [工会所在群号] [指定boss编号]】创建新的总伤害统计表
        ///工会所在群号可以缺省
        static void Main(string[] args)
        {
            Console.WriteLine($"clanUtils会战出刀统计工具 by饼干\r\nVersion {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            if (args.Length < 1) return;
            GetTotalDmg(args);
            Console.WriteLine("按下回车退出程序");
            Console.ReadLine();
        }

        struct Member
        {
            public long uid;
            public string name;
            public long total_dmg;   //总伤害
            public long times;       //出刀数量   
        }

        private static void GetTotalDmg(string[] args)
        {
            Console.WriteLine("将伤害进行统计");
            string DBPath = args[0];                        //数据库路径
            string TableName = null;                        //需要进行统计的表名
            string Gid = args.Length > 1 ? args[1] : null;  //所在群号
            List<Member> MemberList = new List<Member>();   //成员列表

            SQLiteConnection SQLConnection = new SQLiteConnection($"DATA SOURCE={DBPath}");
            SQLConnection.Open();

            //检查gid合法性
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                if (!string.IsNullOrEmpty(Gid))//是否存在gid
                {
                    cmd.CommandText = $"SELECT COUNT(*) FROM clan WHERE gid='{Gid}'";
                    if (!Convert.ToBoolean(cmd.ExecuteScalar()))//gid不合法时置空
                    {
                        Gid = null;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("不合法群号请重新选择对应群号");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                if (string.IsNullOrEmpty(Gid))//Gid不存在或不合法被置空
                {
                    //读取群号
                    List<long> gids = new List<long>();
                    List<string> guildName = new List<string>();
                    cmd.CommandText = "SELECT CAST(gid AS INTEGER) AS gid,name FROM clan";
                    SQLiteDataReader gidReader = cmd.ExecuteReader();
                    //将群号和名称存入List
                    while (gidReader.Read())
                    {
                        if (gids.IndexOf(Convert.ToInt64(gidReader["gid"])) == -1) 
                        {
                            gids.Add(Convert.ToInt64(gidReader["gid"]));
                            guildName.Add(gidReader["name"].ToString());
                        }
                    }
                    //只查询到一个群号时直接选择
                    if (gids.Count == 1)
                    {
                        Gid = gids[0].ToString();
                        Console.WriteLine($"只检测刀单一群号，已自动选择群{Gid}");
                    }
                    else
                    {
                        if (gids.Count == 0) 
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("弟啊你这库里啥都没有啊（半恼）");
                            Console.ForegroundColor = ConsoleColor.White;
                            return;
                        }
                        Console.WriteLine("选择工会对应群号的编号：");
                        foreach (long gid in gids)
                        {
                            Console.WriteLine($"{gids.IndexOf(gid)} - {gid} - {guildName[gids.IndexOf(gid)]}");
                        }
                        int getIndex = -1;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        while (getIndex < 0 || getIndex > gids.Count - 1)
                        {
                            if (int.TryParse(Console.ReadLine(), out getIndex))//尝试转换输入
                            {
                                //检查输入数值合法性
                                if (getIndex < 0 || getIndex > gids.Count - 1) Console.WriteLine("弟啊你选了啥啊,重新写个编号");
                            }
                            else 
                            {
                                Console.WriteLine("弟啊你这不是数字啊,重新写个编号");
                                getIndex = -1;
                            }
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Gid = gids[getIndex].ToString();
                    }
                }
            }

            //读取所有成员的uid
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                Console.Write("获取用户数据.....");
                //创建伤害统计表
                cmd.CommandText = $"SELECT CAST(uid AS INTEGER) AS uid,name FROM member WHERE gid={Gid}";
                cmd.ExecuteNonQuery();
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Member member = new Member();
                    member.uid = Convert.ToInt64(dataReader["uid"]);
                    member.name = dataReader["name"].ToString();
                    member.times = 0;
                    MemberList.Add(member);
                }
                if(MemberList.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("未查找到公会存在任意成员");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                Console.WriteLine("成功");
            }

            //自动查询数据库出刀表
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                cmd.CommandText = "SELECT * FROM sqlite_master WHERE type='table'";
                SQLiteDataReader memberDataReader = cmd.ExecuteReader();
                List<string> tables = new List<string>();//用于存储表格名
                while (memberDataReader.Read())
                {
                    if (memberDataReader["tbl_name"].ToString().Length >= 6)
                    {
                        //找出带有battle前缀的表
                        if (memberDataReader["tbl_name"].ToString().Substring(0, 6).Equals("battle"))
                        {
                            //筛选出相同群号的表
                            string currentTable = memberDataReader["tbl_name"].ToString();
                            if ((currentTable.Split(new char[] { '_' })[1]).Equals(Gid))
                                tables.Add(memberDataReader["tbl_name"].ToString());
                        }
                    }
                }
                if(tables.Count==0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("ERROR:未找到会战相关表格");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                List<int> dates = new List<int>();//用于存储表格名的日期
                foreach (string tableName in tables)
                {
                    dates.Add(Convert.ToInt32(tableName.Split(new char[] { '_' })[3]));
                }
                int nameIndex = dates.IndexOf(dates.Max());//找出最新的日期
                TableName = tables[nameIndex];
            }

            //询问表格的导出方式
            Console.WriteLine(  "选择一个导出方式\r\n" +
                                "1 - 导出到数据库(创建新的表)\r\n" +
                                "2 - 导出为Excel表格(xls文件)");
            int getOutType = 0;
            Console.ForegroundColor = ConsoleColor.Yellow;
            while (getOutType <= 0 || getOutType >= 3)
            {
                if (int.TryParse(Console.ReadLine(), out getOutType))//尝试转换输入
                {
                    //检查输入数值合法性
                    if (getOutType < 0 || getOutType > 2) Console.WriteLine("弟啊你选了啥啊,重新选一个");
                }
                else
                {
                    Console.WriteLine("弟啊你这不是数字啊 ¿");
                }
            }
            Console.ForegroundColor = ConsoleColor.White;

            //判断是否指定统计单一boss
            int bossID = 0;
            if (args.Length == 3) int.TryParse(args[2], out bossID);
            if (bossID == 0 || bossID > 5)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("BossID不合法，统计全部数据");
                bossID = 0;
                Console.ForegroundColor = ConsoleColor.White;
            }

            //判断输出类型为数据库
            if (getOutType == 1)
            {
                //查找是否已经创建过统计表,如有则删除
                using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
                {
                    cmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='members_total_dmg_{DateTime.Today.Year}{DateTime.Today.Month}'";
                    if (Convert.ToBoolean(cmd.ExecuteScalar()))
                    {
                        cmd.CommandText = "DROP TABLE members_total_dmg_" +
                                                        DateTime.Today.Year + DateTime.Today.Month;
                        cmd.ExecuteNonQuery();
                    }
                }
                //创建伤害统计表
                using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
                {
                    cmd.CommandText = $"CREATE TABLE members_total_dmg_{DateTime.Today.Year}{DateTime.Today.Month}" +
                                        " (" +
                                        "uid INTEGER PRIMARY KEY NOT NULL," +
                                        "name VARCHAT NOT NULL," +
                                        "total_dmg INTEGER NOT NULL," +
                                        "avg_dmg INTEGER NOT NULL," +
                                        "dmg_times INTEGER NOT NULL" +
                                        ")";
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("开始计算伤害总和");
            if (bossID != 0) Console.WriteLine($"只统计对{bossID}王的伤害");
            //统计伤害
            for (int i= 0; i < MemberList.Count; i++)
            {
                Console.WriteLine("============================");
                using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
                {
                    cmd.CommandText = $"SELECT * FROM {TableName}" +
                                    $" WHERE uid='{MemberList[i].uid}'";
                    if (bossID != 0) cmd.CommandText += $" AND boss='{bossID}'";

                    //读取相应uid的伤害数据
                    Member member = MemberList[i];//取出当前索引的成员
                    Console.WriteLine($"正在计算  {MemberList[i].name}  的伤害");
                    SQLiteDataReader memberDataReader = cmd.ExecuteReader();
                    while (memberDataReader.Read())
                    {
                        member.times++;
                        member.total_dmg += Convert.ToInt32(memberDataReader["dmg"]);
                    }
                    MemberList[i] = member;//更新数据
                }
                Console.WriteLine($"得到  {MemberList[i].name}  的总伤害为  {MemberList[i].total_dmg}  ({MemberList[i].times})");
                if (getOutType == 1)//判断输出类型为数据库，向统计表插入新行
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
                    {
                        cmd.CommandText = "INSERT INTO members_total_dmg_" +
                                    DateTime.Today.Year + DateTime.Today.Month +
                                    " (uid,name,total_dmg,avg_dmg,dmg_times) VALUES (" +
                                    $"'{MemberList[i].uid}'," +
                                    $"'{MemberList[i].name}'," +
                                    $"'{MemberList[i].total_dmg}'," +
                                    $"'{(MemberList[i].total_dmg / (MemberList[i].times == 0 ? 1 : MemberList[i].times))}'," +
                                    $"'{MemberList[i].times}'" +
                                    ")";
                        cmd.ExecuteNonQuery();
                    }
                }                
            }
            SQLConnection.Close();//关闭数据库连接

            if (getOutType == 2)//判断输出类型为Excel，并写入Excel
            {
                HSSFWorkbook dmgExcel = new HSSFWorkbook();//新建一个excel
                dmgExcel.CreateSheet("公会战伤害统计"); //新建一个工作表
                ISheet dmgSheet = dmgExcel.GetSheet("公会战伤害统计");
                dmgSheet.CreateRow(0);

                //写入第一行数据
                IRow firstRow = dmgSheet.GetRow(0);
                firstRow.CreateCell(0).SetCellValue("QQ");
                firstRow.CreateCell(1).SetCellValue("昵称");
                firstRow.CreateCell(2).SetCellValue("总伤害");
                firstRow.CreateCell(3).SetCellValue("平均伤害");
                firstRow.CreateCell(4).SetCellValue("出刀次数");

                //写入伤害数据
                int rowNum = 1;
                foreach (Member member in MemberList)
                {
                    dmgSheet.CreateRow(rowNum);//创建行
                    IRow sheetRow = dmgSheet.GetRow(rowNum); // 获得行索引
                    sheetRow.CreateCell(0).SetCellValue(Convert.ToDouble(member.uid));
                    sheetRow.CreateCell(1).SetCellValue(member.name);
                    sheetRow.CreateCell(2).SetCellValue(member.total_dmg);
                    sheetRow.CreateCell(3).SetCellValue(member.total_dmg / (member.times == 0 ? 1 : member.times));//这里使用三元符号防止除0
                    sheetRow.CreateCell(4).SetCellValue(member.times);
                    rowNum++;
                }
                //设置自动宽度
                dmgSheet.AutoSizeColumn(0);
                dmgSheet.AutoSizeColumn(1);
                dmgSheet.AutoSizeColumn(2);
                dmgSheet.AutoSizeColumn(3);
                dmgSheet.AutoSizeColumn(4);
                try 
                {
                    FileStream stream = new FileStream(@"伤害统计表.xls", FileMode.Create);
                    dmgExcel.Write(stream);
                    stream.Close();
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e);
                    Console.WriteLine("写入表格文件时发生错误");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("伤害统计完成");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
