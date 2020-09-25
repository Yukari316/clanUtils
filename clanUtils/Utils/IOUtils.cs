using System;
using System.IO;
using System.Runtime.InteropServices;
using NPOI.XSSF.UserModel;

namespace clanUtils.Utils
{
    internal static class IOUtils
    {
        #region Win32 API
        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private const           int    OF_READWRITE       = 2;
        private const           int    OF_SHARE_DENY_NONE = 0x40;
        private static readonly IntPtr HFILE_ERROR        = new IntPtr(-1);
        #endregion

        private static void CheckFileInUse()
        {
            //检测文件是否被占用
            IntPtr vHandle = _lopen("伤害统计表.xlsx", OF_READWRITE | OF_SHARE_DENY_NONE);
            while (vHandle == HFILE_ERROR)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("文件被占用，按下回车重试");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadLine();
                Console.WriteLine("开始重试写入");
                vHandle = _lopen("伤害统计表.xlsx", OF_READWRITE | OF_SHARE_DENY_NONE);
            }
            //关闭文件
            CloseHandle(vHandle);
        }

        public static XSSFWorkbook OpenOrCreateDmgWorkbook(string path = null)
        {
            if (string.IsNullOrEmpty(path)) path = "伤害统计表.xlsx";
            //初始化表格
            if (File.Exists(path))//存在时读取
            {
                CheckFileInUse();
                using FileStream excelFile = new FileStream(path, FileMode.Open);
                return new XSSFWorkbook(excelFile);
            }
            //不存在时创建
            else return new XSSFWorkbook();
        }

        public static void WriteToWorkbookFile(XSSFWorkbook dmgWorkbook, string path = null)
        {
            if (string.IsNullOrEmpty(path)) path = "伤害统计表.xlsx";
            using FileStream excelFile           = new FileStream(path, FileMode.Create);
            //写入数据
            dmgWorkbook.Write(excelFile);
            excelFile.Close();
        }
    }
}