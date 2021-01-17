using System;
using System.Text;
using System.Threading;

namespace clanUtils.Utils
{
    internal static class ConsoleLog
    {
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
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[INFO][{type}]{message}");
        }

        /// <summary>
        /// 向控制台发送Error信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        public static void Error(object type, object message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"][{type}]");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// 向控制台发送Fatal信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="message">信息内容</param>
        private static void Fatal(object type, object message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("FATAL");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"][{type}]");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message.ToString());
            Console.ForegroundColor = ConsoleColor.White;
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
