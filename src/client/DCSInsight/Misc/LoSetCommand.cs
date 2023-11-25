using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace DCSInsight.Misc
{
    internal class LoSetCommand
    {
        public string Description { get; private set; }
        public string Code { get; private set; }

        internal static List<LoSetCommand> LoadCommands()
        {
            var result = new List<LoSetCommand>();
            var resourceStream = Application.GetResourceStream(new Uri(@"/dcs-insight;component/Items/iCommands.txt", UriKind.Relative));
            if (resourceStream == null) return result;

            var streamReader = new StreamReader(resourceStream.Stream);
            string line;

            while ((line = streamReader.ReadLine()) != null)
            {
                if(!line.Trim().StartsWith("i") || !line.Contains('\t') || line.Contains('/')) continue;

                var array = line.Split('\t');
                result.Add(new LoSetCommand{Code = array[1].Trim(), Description = array[0].Trim() });
            }

            result = result.OrderBy(o => o.Description).ToList();
            return result;
        }
    }
}
