using System;
using System.Collections.Generic;
using clanUtils.Res;
using clanUtils.Utils;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace clanUtils.Program
{
    internal static class WorkBookParse
    {
        public static void GenerateDmgWorkbook(List<AtkData> atkDatas, string name)
        {
            string       sheetName   = $"{name}_{DateTime.Today.Year}-{DateTime.Today.Month}-{DateTime.Today.Day}";
            XSSFWorkbook dmgWorkbook = IOUtils.OpenOrCreateDmgWorkbook();
            if (dmgWorkbook.GetSheetIndex(sheetName) != -1)
            {
                ConsoleLog.Error("Excel生成","今天已进行过统计，删除旧表");
                dmgWorkbook.RemoveSheetAt(dmgWorkbook.GetSheetIndex(sheetName));
            }

            ISheet dmgSheet = dmgWorkbook.CreateSheet(sheetName); //新建一个工作表
            dmgSheet.CreateRow(0);

            //写入第一行数据
            IRow firstRow = dmgSheet.GetRow(0);
            int  rowCount = 0;
            firstRow.CreateCell(rowCount++).SetCellValue("QQ");
            firstRow.CreateCell(rowCount++).SetCellValue("昵称");
            firstRow.CreateCell(rowCount++).SetCellValue("总伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("平均伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("出刀次数");
            firstRow.CreateCell(rowCount++).SetCellValue("RSD(%)");

            firstRow.CreateCell(rowCount++).SetCellValue("对一王总伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对一王平均伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对一王出刀次数");
            firstRow.CreateCell(rowCount++).SetCellValue("RSD(%)");

            firstRow.CreateCell(rowCount++).SetCellValue("对二王总伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对二王平均伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对二王出刀次数");
            firstRow.CreateCell(rowCount++).SetCellValue("RSD(%)");

            firstRow.CreateCell(rowCount++).SetCellValue("对三王总伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对三王平均伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对三王出刀次数");
            firstRow.CreateCell(rowCount++).SetCellValue("RSD(%)");

            firstRow.CreateCell(rowCount++).SetCellValue("对四王总伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对四王平均伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对四王出刀次数");
            firstRow.CreateCell(rowCount++).SetCellValue("RSD(%)");

            firstRow.CreateCell(rowCount++).SetCellValue("对五王总伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对五王平均伤害");
            firstRow.CreateCell(rowCount++).SetCellValue("对五王出刀次数");
            firstRow.CreateCell(rowCount).SetCellValue("RSD(%)");

            //写入伤害数据
            int rowNum = 1;

            foreach (AtkData atkData in atkDatas)
            {
                int colNum = 0;
                dmgSheet.CreateRow(rowNum);              //创建行
                IRow sheetRow = dmgSheet.GetRow(rowNum); // 获得行索引
                //总伤害数据
                sheetRow.CreateCell(colNum++).SetCellValue(atkData.Uid.ToString());
                sheetRow.CreateCell(colNum++).SetCellValue(atkData.Name);
                sheetRow.CreateCell(colNum++).SetCellValue(atkData.TotalDmg);
                sheetRow.CreateCell(colNum++).SetCellValue(double.IsNaN(atkData.TotalAvgDmg) ? 0 : atkData.TotalAvgDmg);
                sheetRow.CreateCell(colNum++).SetCellValue(atkData.TotalTimes);
                sheetRow.CreateCell(colNum++).SetCellValue(atkData.Deviation);
                //boss伤害数据
                foreach (BossDmg bossDmg in atkData.BossDmgInfos)
                {
                    sheetRow.CreateCell(colNum++).SetCellValue(bossDmg.Dmg);
                    sheetRow.CreateCell(colNum++).SetCellValue(double.IsNaN(bossDmg.AvgDmg) ? 0 : bossDmg.AvgDmg);
                    sheetRow.CreateCell(colNum++).SetCellValue(bossDmg.Count);
                    sheetRow.CreateCell(colNum++).SetCellValue(bossDmg.Deviation);
                }
                rowNum++;
            }
            //设置自动宽度
            for (int i = 0; i < 20; i++)
            {
                dmgSheet.AutoSizeColumn(i);
            }
            //写入数据到文件
            IOUtils.WriteToWorkbookFile(dmgWorkbook);
        }
    }
}
