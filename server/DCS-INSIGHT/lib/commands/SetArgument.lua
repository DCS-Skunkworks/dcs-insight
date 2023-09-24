module("SetArgument", package.seeall)

local APIInfo = require("APIInfo")
local Parameter = require("Parameter")

-- This is the unique ID for this particular API
local API_ID = 1

--- @class SetArgument : APIHandlerBase
--- @field id number API ID
--- @field apiInfo APIInfo
local SetArgument = {}


--- @func Returns new SetArgument
function SetArgument:new()
    local apiInfo = APIInfo:new()
    apiInfo.id = API_ID
    apiInfo.returns_data = false
    apiInfo.api_syntax = "GetDevice(device_id):set_argument_value(argument_id, new_value)"
    apiInfo.parameter_count = 3
    apiInfo.parameter_defs = {}
    local parameter0 = Parameter:new(0, ParamName.device_id, ParamType.number)
    apiInfo.parameter_defs[#apiInfo.parameter_defs + 1] = parameter0

    local parameter1 = Parameter:new(1, ParamName.argument_id, ParamType.number)
    apiInfo.parameter_defs[#apiInfo.parameter_defs + 1] = parameter1

    local parameter2 = Parameter:new(2, ParamName.new_value, ParamType.number)
    apiInfo.parameter_defs[#apiInfo.parameter_defs + 1] = parameter2

    --- @type SetArgument
    local o = {
        id = API_ID,
        apiInfo = apiInfo
    }
    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function SetArgument:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function SetArgument:execute(api)
    local param0
    local param1
    local param2

    for i, param in pairs(api.parameter_defs) do
        if (param.id == 0) then param0 = param.value end
        if (param.id == 1) then param1 = param.value end
        if (param.id == 2) then param2 = param.value end
    end

    if(param0 == nil or param1 == nil or param2 == nil) then 
        api.result = "Invalid parameter values." 
    else        
        api.result = GetDevice(param0):set_argument_value(param1, param2)        
        if(api.result == nil) then api.result = "API called, result is nil" end
        Logg.log("Result was "..api.result)
    end
    
    return api
end

return SetArgument
