using System;
using System.Text;
using System.Threading;
using Konsole;

namespace clanUtils.Utils
{
    internal static class ConsoleLog
    {
        #region 控制台输出控制

        private static Window logWindow = new Window(Console.WindowWidth, Console.WindowHeight - 2);

        private static IConsole logConsole =
            logWindow.OpenBox("Log", 15, 0, Console.WindowWidth - 15, Console.WindowHeight - 2);
        internal static IConsole statusConsole =
            logWindow.OpenBox("统计信息", 0, 0, 15, Console.WindowHeight - 2);
        #endregion

        #region 格式化错误Log
        public static string ErrorLogBuilder(Exception e)
        {
            StringBuilder errorMessageBuilder = new StringBuilder();
            errorMessageBuilder.Append("\r\n");
            errorMessageBuilder.Append("==============ERROR==============\r\n");
            errorMessageBuilder.Append("Error:");
            errorMessageBuilder.Append(e.GetType().FullName);
            errorMessageBuilder.Append("\r\n\r\n");
            errorMessageBuilder.Append("Message:");
            errorMessageBuilder.Append(e.Message);
            errorMessageBuilder.Append("\r\n\r\n");
            errorMessageBuilder.Append("Stack Trace:\r\n");
            errorMessageBuilder.Append(e.StackTrace);
            errorMessageBuilder.Append("\r\n");
            errorMessageBuilder.Append("=================================\r\n");
            return errorMessageBuilder.ToString();
        }
        #endregion

        /// <summary>
        /// 向控制台发送Info信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Info(object type, object message)
        {
            logConsole.ForegroundColor = ConsoleColor.White;
            logConsole.WriteLine($"[INFO][{type}]{message}");
        }

        /// <summary>
        /// 向控制台发送Error信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Error(object type, object message)
        {
            logConsole.ForegroundColor = ConsoleColor.White;
            logConsole.Write("[");
            logConsole.ForegroundColor = ConsoleColor.Red;
            logConsole.Write("ERROR");
            logConsole.ForegroundColor = ConsoleColor.White;
            logConsole.Write($"][{type}]");
            logConsole.ForegroundColor = ConsoleColor.Red;
            logConsole.WriteLine(message.ToString());
            logConsole.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 向控制台发送Fatal信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        private static void Fatal(object type, object message)
        {
            logConsole.ForegroundColor = ConsoleColor.White;
            logConsole.Write("[");
            logConsole.ForegroundColor = ConsoleColor.DarkRed;
            logConsole.Write("FATAL");
            logConsole.ForegroundColor = ConsoleColor.White;
            logConsole.Write($"][{type}]");
            logConsole.ForegroundColor = ConsoleColor.DarkRed;
            logConsole.WriteLine(message.ToString());
            logConsole.ForegroundColor = ConsoleColor.White;
        }

        #region 全局错误Log
        /// <summary>
        /// 全局错误Log
        /// </summary>
        /// <param name="e"></param>
        public static void UnhandledExceptionLog(Exception e)
        {
            Fatal("Fatal Exception",ErrorLogBuilder(e));
            Thread.Sleep(5000);
            Environment.Exit(0);
        }
        #endregion
    }
}
