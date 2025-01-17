﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SushiConsoleDev.Logger
{
    public static class Logger
    {
        public static int fileSize = 30720;
        public static string _dateTime = String.Format("{0:yyyyMMdd}_", DateTime.UtcNow);
        public static string targetName = default;
        public static string path = default;
        public static string pathDirectory = GetPath();

        public static int Counter { get; private set; } = 1;

        public static int ThreadInfo { get; private set; } = Thread.CurrentThread.ManagedThreadId;

        public static void Info(Type type, string method, string message)
        {
            LoggerHelper(LoggerLayers.Info.ToString(), type, method, message, ThreadInfo.ToString());
        }

        public static void Debug(Type type, string method, string message)
        {
            LoggerHelper(LoggerLayers.Debug.ToString(), type, method, message, ThreadInfo.ToString());
        }
     
        public static void Error(Type type, string method, string message)
        {
            LoggerHelper(LoggerLayers.Error.ToString(), type, method, message, ThreadInfo.ToString());
        }

        public static void CheckFileSize()
        {
            string[] filePaths = Directory.GetFiles(pathDirectory);
            List<string> filePathsCollection = new List<string>();
            List<int> filePathParsedNumbers = new List<int>();

            foreach (var item in filePaths)
            {
                int index = item.IndexOf('[');
                var nameToParse = item.Substring(index + 1);
                var targetNumberString = new String(nameToParse.Where(Char.IsDigit).ToArray());
                var targetNumber = int.Parse(targetNumberString);

                filePathParsedNumbers.Add(targetNumber);
            }
            filePathParsedNumbers.Sort();

            if (filePathParsedNumbers.Count == 0)
            {
                path = GetPath() + $"\\log{_dateTime}[{Counter}].txt";
            }
            else
            {
                targetName = $"log{_dateTime}[{filePathParsedNumbers[filePathParsedNumbers.Count - 1]}].txt";

                var newPathToTagretFile = @$"{pathDirectory}" + $"\\{targetName}";
                FileInfo file = new System.IO.FileInfo(newPathToTagretFile);
                long size = file.Length;
                
                if (size > fileSize)
                {
                    Counter = filePathParsedNumbers[filePathParsedNumbers.Count - 1];
                    ++Counter;
                    path = GetPath() + $"\\log{_dateTime}[{Counter}].txt";
                }
                else
                {
                    path = newPathToTagretFile;
                }
            }
        }
        public static async void LoggerHelper(string layer, Type type, string method, string message, string threadInfo)
        {
            CheckFileSize();
            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                StringBuilder sb = new StringBuilder();
                var time = String.Format("{0:HH:mm:ss dd/MM/yyyy} ", DateTime.Now);
                var fromNamespaceType = type.ToString();
                sb.Append(time);
                sb.Append($"[{layer}] ");
                sb.Append($"{fromNamespaceType}" + $".{method}_");
                sb.Append($"{message}_");
                sb.Append($"thread id:{threadInfo}");
                sb.ToString();

                await streamWriter.WriteLineAsync(sb.ToString());
            }
        }

        public static string GetPath()
        {
            var path = Environment.CurrentDirectory + "\\LoggerRepository";
            return path;
        }
    }
}