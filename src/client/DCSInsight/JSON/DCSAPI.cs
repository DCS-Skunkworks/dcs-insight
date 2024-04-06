using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace DCSInsight.JSON
{
    [Serializable]
    public class DCSAPI
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [JsonConstructor]
        public DCSAPI(int id, 
            bool returns_data, 
            string api_syntax, 
            int parameter_count, 
            List<ParameterInfo> parameter_defs, 
            bool error_thrown, 
            string? error_message, 
            string? result, 
            string? result_type)
        {
            Id = id;
            ReturnsData = returns_data;
            Syntax = api_syntax;
            ParamCount = parameter_count;
            Parameters = parameter_defs;
            ErrorThrown = error_thrown;
            ErrorMessage = error_message;
            Result = result;
            ResultType = result_type;
        }

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

        [JsonProperty("error_thrown", Required = Required.Default)]
        public bool ErrorThrown { get; set; }

        [JsonProperty("error_message", Required = Required.Default)]
        public string? ErrorMessage { get; set; }

        [JsonProperty("result", Required = Required.Default)]
        public string? Result { get; set; }

        [JsonProperty("result_type", Required = Required.Default)]
        public string? ResultType { get; set; }
    }


    public enum ParameterTypeEnum
    {
        // ReSharper disable once InconsistentNaming
        number = 0,
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
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
        public ParameterInfo(int id, string parameterName, string? value, ParameterTypeEnum type)
        {
            Id = id;
            ParameterName = parameterName;
            Value = value;
            Type = type;
        }

        [JsonProperty("id", Required = Required.Default)]
        public int Id { get; set; }

        [JsonProperty("name", Required = Required.Default)]
        public string ParameterName { get; set; }

        [JsonProperty("value", Required = Required.Default)]
        public string? Value { get; set; }

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
