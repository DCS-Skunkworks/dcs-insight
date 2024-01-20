using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DCSInsight.Lua
{
    internal static class LuaAssistant
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly object LockObject = new();
        private static string _dcsbiosAircraftsLuaLocation;
        private static string _dcsbiosModuleLuaFilePath;
        private static readonly List<KeyValuePair<string, string>> LuaControls = new();
        private static string _aircraftId = "";
        private const string DCSBIOS_LUA_NOT_FOUND_ERROR_MESSAGE = "Error loading DCS-BIOS lua.";

        internal static string GetLuaCommand(string controlId)
        {
            if (_aircraftId == null || string.IsNullOrEmpty(controlId)) return "";

            if (LuaControls.Count == 0)
            {
                ReadLuaCommandsFromFile();
                if (LuaControls.Count == 0) return "";
            }

            var result = LuaControls.Find(o => o.Key == controlId);
            if (result.Key != controlId) return "";

            return result.Value;
        }

        internal static List<string> GetAircraftList(string dcsbiosJSONPath)
        {
            _dcsbiosAircraftsLuaLocation = $"{dcsbiosJSONPath}\\..\\..\\lib\\modules\\aircraft_modules\\";
            _dcsbiosModuleLuaFilePath = $"{dcsbiosJSONPath}\\..\\..\\lib\\modules\\Module.lua";
            var directoryInfo = new DirectoryInfo(_dcsbiosAircraftsLuaLocation);
            IEnumerable<FileInfo> files;
            try
            {
                files = directoryInfo.EnumerateFiles("*.lua", SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to find DCS-BIOS lua files. -> {Environment.NewLine}{ex.Message}");
            }

            var result = files.Select(file => file.Name.Replace(".lua", "")).ToList();
            result.Remove("CommonData");
            result.Remove("MetadataEnd");
            result.Remove("MetadataStart");
            return result;
        }

        private static void ReadControlsFromLua(string aircraftId)
        {
            if (_aircraftId == aircraftId && LuaControls.Count > 0) return;

            _aircraftId = aircraftId;
            LuaControls.Clear();

            // input is a map from category string to a map from key string to control definition
            // we read it all then flatten the grand children (the control definitions)
            var lineArray = File.ReadAllLines(_dcsbiosModuleLuaFilePath + aircraftId + ".lua");
            try
            {
                var luaBuffer = "";

                foreach (var s in lineArray)
                {
                    //s.StartsWith("--") 
                    if (string.IsNullOrEmpty(s)) continue;

                    if (s.StartsWith(aircraftId + ":define"))
                    {
                        luaBuffer = s;

                        if (CountParenthesis(true, luaBuffer) == CountParenthesis(false, luaBuffer))
                        {
                            LuaControls.Add(CopyControlFromLuaBuffer(luaBuffer));
                            luaBuffer = "";
                        }
                    }
                    else if (!string.IsNullOrEmpty(luaBuffer))
                    {
                        //We have incomplete data from previously
                        luaBuffer = luaBuffer + "\n" + s;
                        if (CountParenthesis(true, luaBuffer) == CountParenthesis(false, luaBuffer))
                        {
                            LuaControls.Add(CopyControlFromLuaBuffer(luaBuffer));
                            luaBuffer = "";
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "ReadControlsFromLua : Failed to read DCS-BIOS lua.");
            }
        }

        private static KeyValuePair<string, string> CopyControlFromLuaBuffer(string luaBuffer)
        {
            // We have the whole control
            // F_16C_50:define3PosTumb("MAIN_PWR_SW", 3, 3001, 510, "Electric System", "MAIN PWR Switch, MAIN PWR/BATT/OFF")
            /*
             A_10C:defineString("ARC210_COMSEC_SUBMODE", function()
                return Functions.coerce_nil_to_string(arc_210_data["comsec_submode"])
             end, 5, "ARC-210 Display", "COMSEC submode (PT/CT/CT-TD)")
            */
            var startIndex = luaBuffer.IndexOf("\"", StringComparison.Ordinal);
            var endIndex = luaBuffer.IndexOf("\"", luaBuffer.IndexOf("\"") + 1);
            var controlId = luaBuffer.Substring(startIndex + 1, endIndex - startIndex - 1);

            return new KeyValuePair<string, string>(controlId, luaBuffer);
        }

        private static int CountParenthesis(bool firstParenthesis, string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            var parenthesis = firstParenthesis ? '(' : ')';
            var result = 0;
            var insideQuote = false;

            foreach (var c in s)
            {
                if (c == '"') insideQuote = !insideQuote;

                if (c == parenthesis && !insideQuote) result++;
            }

            return result;
        }

        /// <summary>
        /// Load all lua controls
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void ReadLuaCommandsFromFile()
        {
            LuaControls.Clear();

            try
            {
                lock (LockObject)
                {
                    ReadControlsFromLua(_aircraftId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{DCSBIOS_LUA_NOT_FOUND_ERROR_MESSAGE} ==>[{_dcsbiosModuleLuaFilePath}]<=={Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}
