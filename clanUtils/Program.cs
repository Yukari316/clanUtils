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
        ///使用指令 【clanUtils [数据库路径] [工会所在群号]】创建新的总伤害统计表
        ///工会所在群号可以缺省
        static void Main(string[] args)
        {
            if (args.Length < 1) return;
            GetTotalDmg(args);
        }

        struct Member
        {
            public string uid;
            public string name;
            public int total_dmg;   //总伤害
            public int times;       //出刀数量   
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

            //查找是否已经创建过统计表和临时表,如有则删除
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                cmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='members_total_dmg_{DateTime.Today.Year}{DateTime.Today.Month}'";
                if (Convert.ToBoolean(cmd.ExecuteScalar()))
                {
                    cmd.CommandText = "DROP TABLE members_total_dmg_" +
                                                    DateTime.Today.Year + DateTime.Today.Month;
                    cmd.ExecuteNonQuery();
                }
                cmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='member_data'";
                if (Convert.ToBoolean(cmd.ExecuteScalar()))
                {
                    cmd.CommandText = "DROP TABLE member_data";
                    cmd.ExecuteNonQuery();
                }
            }

            ///API读取INT类型默认为Int32类型会造成溢出
            ///SQLite不能完全支持SQL语句，只能增加字段，不能drop字段
            ///需要重新创建一个字段类型为INTEGER临时表,使API可以读取Int64的数值
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                cmd.CommandText =   "CREATE TABLE member_data(" +
                                    "uid INTEGER NOT NULL," +
                                    "alt INTEGER NOT NULL," +
                                    "name VARCHAR NOT NULL," +
                                    "gid INTEGER NOT NULL," +
                                    "cid INTEGER NOT NULL," +
                                    "PRIMARY KEY(uid,alt))";
                cmd.ExecuteNonQuery();
            }
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                cmd.CommandText = $"INSERT INTO member_data SELECT * FROM member";
                cmd.ExecuteNonQuery();
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

            //检查gid合法性
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                if (!string.IsNullOrEmpty(Gid))//是否存在gid
                {
                    cmd.CommandText = $"SELECT COUNT(*) FROM clan WHERE gid='{Gid}'";
                    if (!Convert.ToBoolean(cmd.ExecuteScalar()))//gid不合法时置空
                    {
                        Gid = null;
                        Console.WriteLine("不合法群号请重新选择对应群号");
                    }
                }
                if (string.IsNullOrEmpty(Gid))//Gid不存在或不合法被置空
                {
                    //读取群号
                    List<string> gids = new List<string>();
                    cmd.CommandText = "SELECT gid FROM clan";
                    SQLiteDataReader gidReader = cmd.ExecuteReader();
                    while (gidReader.Read())
                    {
                        if (gids.IndexOf(gidReader["gid"].ToString()) == -1) 
                        {
                            gids.Add(gidReader["gid"].ToString());
                        }
                    }
                    if (gids.Count == 1) Gid = gids[0];
                    else
                    {
                        if(gids.Count==0)
                        {
                            Console.WriteLine("弟啊你这库里啥都没有啊（半恼）");
                            Console.ReadLine();
                            return;
                        }
                        Console.WriteLine("选择工会对应群号：");
                        foreach (string gid in gids)
                        {
                            Console.WriteLine($"{gids.IndexOf(gid)} - {gid}");
                        }
                        int getIndex = -1;
                        while (getIndex < 0 || getIndex > gids.Count - 1)
                        {
                            if (int.TryParse(Console.ReadLine(), out getIndex))//尝试转换输入
                            {
                                if (getIndex < 0 || getIndex > gids.Count - 1) Console.WriteLine("弟啊你选了啥啊,重新写个编号");
                            }
                            else 
                            {
                                Console.WriteLine("弟啊你这不是数字啊,重新写个编号");
                                getIndex = -1;
                            }
                        }
                        Gid = gids[getIndex];
                    }
                }
            }

            //读取所有成员的uid
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                Console.Write("获取用户数据.....");
                //创建伤害统计表
                cmd.CommandText = $"SELECT * FROM member_data WHERE gid={Gid}";
                cmd.ExecuteNonQuery();
                SQLiteDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Member member = new Member();
                    member.uid = dataReader["uid"].ToString();
                    member.name = dataReader["name"].ToString();
                    member.times = 0;
                    MemberList.Add(member);
                }
                Console.WriteLine("成功");
            }

            //自动查询数据库表
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
                    Console.WriteLine("ERROR:未找到会战相关表格");
                    Console.ReadLine();
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

            Console.WriteLine("开始计算伤害总和");
            //统计伤害
            for (int i= 0; i < MemberList.Count; i++)
            {
                Console.WriteLine();
                using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
                {
                    cmd.CommandText = $"SELECT * FROM {TableName}" +
                                    $" WHERE uid='{MemberList[i].uid}'";

                    //读取相应uid的伤害数据
                    Member member = MemberList[i];
                    Console.WriteLine($"正在计算  {MemberList[i].name}  的伤害");
                    SQLiteDataReader memberDataReader = cmd.ExecuteReader();
                    while (memberDataReader.Read())
                    {
                        member.times++;
                        member.total_dmg += Convert.ToInt32(memberDataReader["dmg"]);
                    }
                    MemberList[i] = member;//更新数据
                }
                //向统计表插入新行
                using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
                {
                    cmd.CommandText = "INSERT INTO members_total_dmg_" +
                                DateTime.Today.Year + DateTime.Today.Month +
                                " (uid,name,total_dmg,avg_dmg,dmg_times) VALUES (" +
                                $"'{MemberList[i].uid}'," +
                                $"'{MemberList[i].name}'," +
                                $"'{MemberList[i].total_dmg}'," +
                                $"'{(MemberList[i].total_dmg / MemberList[i].times)}'," +
                                $"'{MemberList[i].times}'" +
                                ")";
                    Console.WriteLine($"得到  {MemberList[i].name}  的总伤害为  {MemberList[i].total_dmg}  ({MemberList[i].times})");
                    cmd.ExecuteNonQuery();
                }
            }

            //删除临时表
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection))
            {
                cmd.CommandText = "DROP TABLE member_data";
                cmd.ExecuteNonQuery();
            }

            //导出到Excel表格
            using (SQLiteCommand cmd = new SQLiteCommand(SQLConnection)) 
            {
                cmd.CommandText = $"SELECT * FROM members_total_dmg_{DateTime.Today.Year}{DateTime.Today.Month}";
                SQLiteDataReader memberDataReader = cmd.ExecuteReader();

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
                while (memberDataReader.Read())
                {
                    dmgSheet.CreateRow(rowNum);//创建行
                    IRow sheetRow = dmgSheet.GetRow(rowNum); // 获得行索引
                    sheetRow.CreateCell(0).SetCellValue(memberDataReader["uid"].ToString());
                    sheetRow.CreateCell(1).SetCellValue(memberDataReader["name"].ToString());
                    sheetRow.CreateCell(2).SetCellValue(memberDataReader["total_dmg"].ToString());
                    sheetRow.CreateCell(3).SetCellValue(memberDataReader["avg_dmg"].ToString());
                    sheetRow.CreateCell(4).SetCellValue(memberDataReader["dmg_times"].ToString());
                    rowNum++;
                }
                //设置自动宽度
                dmgSheet.AutoSizeColumn(0);
                dmgSheet.AutoSizeColumn(1);
                dmgSheet.AutoSizeColumn(2);
                dmgSheet.AutoSizeColumn(3);
                dmgSheet.AutoSizeColumn(4);
                FileStream stream = new FileStream(@"伤害统计表.xls", FileMode.Create);
                dmgExcel.Write(stream);
                stream.Close();
            }
            SQLConnection.Close();
            Console.WriteLine("伤害统计完成");
            Console.ReadLine();
        }
    }
}
