using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DCSInsight.JSON
{
    public class DCSAPI
    {
        /// <summary>
        /// This is a base class for the DCS-BIOS Control as specified in lua / JSON.
        /// This class is used when reading the JSON.
        /// </summary>


        [JsonProperty("id", Required = Required.Default)]
        public int Id { get; set; }

        [JsonProperty("returns_data", Required = Required.Default)]
        public bool ReturnsData { get; set; }

        [JsonProperty("api_syntax", Required = Required.Default)]
        public string Syntax { get; set; }

        [JsonProperty("parameter_count", Required = Required.Default)]
        public int ParamCount { get; set; }

        [JsonProperty("parameter_defs", Required = Required.Default)]
        public List<ParameterInfo> Parameters { get; set; }

        [JsonProperty("result", Required = Required.Default)]
        public string Result { get; set; }
    }


    public enum ParameterTypeEnum
    {
        number = 0,
        str = 1
    }

    public enum ParamNameEnum
    {
        DeviceId = 0,
        CommandId = 1,
        ArgumentId = 2,
        NewValue = 3
    }


    public class ParameterInfo
    {
        [JsonProperty("id", Required = Required.Default)]
        public int Id { get; set; }

        [JsonProperty("name", Required = Required.Default)]
        public string ParameterName { get; set; }

        [JsonProperty("value", Required = Required.Default)]
        public string Value { get; set; }

        [JsonProperty("type", Required = Required.Default)]
        public ParameterTypeEnum Type { get; set; }


        public int GetParameterNameId()
        {
            return ParameterName switch
            {
                "device_id" => 0,
                "command_id" => 1,
                "argument_id" => 2,
                "new_value" => 3,
                _ => throw new Exception($"Failed to find id for {ParameterName}.")
            };
        }
    }

}
