using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clanUtils
{
    internal class ExcelParse
    {
        public static void SheetExistsParse(IWorkbook workbook,string sheetName)
        {
            //获取所有表格的名称
            List<string> sheetNames = new List<string>();
            foreach (ISheet sheet in workbook)
            {
                sheetNames.Add(sheet.SheetName);
            }
            
            //查找是否有重复表
            if (sheetNames.Where(name => name == sheetName).Count() >= 1)
            {
                workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("发现今天已进行过统计，删除旧表");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void GenerateDMGSheet(ref IWorkbook dmgExcel, List<Member> MemberList, string sheetName)
        {
            ISheet dmgSheet = dmgExcel.CreateSheet(sheetName); //新建一个工作表
            dmgSheet.CreateRow(0);

            //写入第一行数据
            IRow firstRow = dmgSheet.GetRow(0);
            firstRow.CreateCell(0).SetCellValue("QQ");
            firstRow.CreateCell(1).SetCellValue("昵称");
            firstRow.CreateCell(2).SetCellValue("总伤害");
            firstRow.CreateCell(3).SetCellValue("平均伤害");
            firstRow.CreateCell(4).SetCellValue("出刀次数");

            firstRow.CreateCell(5).SetCellValue("对一王总伤害");
            firstRow.CreateCell(6).SetCellValue("对一王平均伤害");
            firstRow.CreateCell(7).SetCellValue("对一王出刀次数");

            firstRow.CreateCell(8).SetCellValue("对二王总伤害");
            firstRow.CreateCell(9).SetCellValue("对二王平均伤害");
            firstRow.CreateCell(10).SetCellValue("对二王出刀次数");

            firstRow.CreateCell(11).SetCellValue("对三王总伤害");
            firstRow.CreateCell(12).SetCellValue("对三王平均伤害");
            firstRow.CreateCell(13).SetCellValue("对三王出刀次数");

            firstRow.CreateCell(14).SetCellValue("对四王总伤害");
            firstRow.CreateCell(15).SetCellValue("对四王平均伤害");
            firstRow.CreateCell(16).SetCellValue("对四王出刀次数");

            firstRow.CreateCell(17).SetCellValue("对五王总伤害");
            firstRow.CreateCell(18).SetCellValue("对五王平均伤害");
            firstRow.CreateCell(19).SetCellValue("对五王出刀次数");

            //写入伤害数据
            int rowNum = 1;

            foreach (Member member in MemberList)
            {
                int colNum = 0;
                dmgSheet.CreateRow(rowNum);//创建行
                IRow sheetRow = dmgSheet.GetRow(rowNum); // 获得行索引

                sheetRow.CreateCell(colNum++).SetCellValue(Convert.ToDouble(member.uid));
                sheetRow.CreateCell(colNum++).SetCellValue(member.name);
                sheetRow.CreateCell(colNum++).SetCellValue(member.total_dmg);
                sheetRow.CreateCell(colNum++).SetCellValue(member.total_dmg / (member.times == 0 ? 1 : member.times));//这里使用三元符号防止除0
                sheetRow.CreateCell(colNum++).SetCellValue(member.times);
                for (int i = 0; i < 5; i++)
                {
                    sheetRow.CreateCell(colNum++).SetCellValue((long)member.GetType().GetProperty($"boss{i + 1}_dmg").GetValue(member)); ;
                    sheetRow.CreateCell(colNum++).SetCellValue((long)member.GetType().GetProperty($"boss{i + 1}_avg_dmg").GetValue(member));
                    sheetRow.CreateCell(colNum++).SetCellValue((int)member.GetType().GetProperty($"boss{i + 1}_times").GetValue(member));
                }
                rowNum++;
            }
            //设置自动宽度
            for (int i = 0; i < 20; i++)
            {
                dmgSheet.AutoSizeColumn(i);
            }
        }
    }
}
