using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DCSInsight.Misc
{
    internal class LoSetCommand
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string CommandsFile = "iCommands.txt";

        public string Description { get; private set; }
        public string Code { get; private set; }

        internal static List<LoSetCommand> LoadCommands()
        {
            var result = new List<LoSetCommand>();
            var loSetCommandsFile = $"{AppDomain.CurrentDomain.BaseDirectory}{CommandsFile}";
            if (!File.Exists(loSetCommandsFile))
            {
                Logger.Error($"Failed to find {CommandsFile} in base directory.");
                return result;
            }

            var stringArray = File.ReadAllLines(loSetCommandsFile);

            foreach (var s in stringArray)
            {
                if (string.IsNullOrEmpty(s) || !s.Trim().StartsWith("i") || !s.Contains('\t') || s.Contains('/') || s.Contains(':')) continue;

                var array = s.Split('\t');
                result.Add(new LoSetCommand { Code = array[1].Trim(), Description = array[0].Trim() });
            }

            result = result.OrderBy(o => o.Description).ToList();
            return result;
        }
    }
}
