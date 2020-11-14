using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DebuggingLog
{
    public enum MLevel
    {
        Message,
        Warning,
        Error,
        Success
    }

    public class Debugger
    {
        static bool printing = false;
        static bool Logging = false;
        static bool acceptPrinting = true;
        static List<Tuple<object, MLevel>> Messages = new List<Tuple<object, MLevel>>();

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if(Logging)
            {
                if (File.Exists(@".\log.txt"))
                    File.Delete(@".\log.txt");
                PrintToConsole("Writing to logs", MLevel.Warning);
                acceptPrinting = false;
                foreach (Tuple<object, MLevel> msg in Messages)
                {
                    PrintToFile(msg.Item1, msg.Item2, "log.txt");
                }
            }
        }

        public static void TurnLogs()
        {
            PrintToConsole("Turned logger on", MLevel.Success);
            Logging = !Logging;
            if (Logging)
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }


        public static void PrintToConsole<T>(T msg, MLevel Level)
        {
            if(acceptPrinting)
            {
                do
                {
                    Thread.Sleep(1);
                } while (printing);
                printing = true;
                switch (Level)
                {
                    case MLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case MLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case MLevel.Success:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }
                Console.WriteLine(msg);
                if(Logging)
                {
                    Tuple<object, MLevel> msgTuple = new Tuple<object, MLevel>(msg, Level);
                    Messages.Add(msgTuple);
                }
                Console.ResetColor();
                printing = false;
            }
        }

        public static void PrintToFile<T>(T msg, MLevel level, string FileName)
        {
            string path = Path.GetFullPath(@"./" + FileName);
            using(StreamWriter sw = new StreamWriter(path, true))
            {
                string Level = "Message";
                switch(level)
                {
                    case MLevel.Success:
                        Level = "Success";
                        break;
                    case MLevel.Warning:
                        Level = "Warning";
                        break;
                    case MLevel.Error:
                        Level = "Error";
                        break;
                }
                sw.WriteLine("[" + Level + "]: " + msg);
                sw.Close();
            }
        }
    }
}
