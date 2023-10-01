module("GetFrequency", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 4

--- @class GetFrequency : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local GetFrequency = APIBase:new()


--- @func Returns new GetFrequency
function GetFrequency:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "GetDevice(device_id):get_frequency()",
        1
    )

    o:add_param_def(0, ParamName.device_id, ParamType.number)

    setmetatable(o, self)
    self.__index = self

    return o
end

--- @func Inits with internal data
function GetFrequency:init()    
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function GetFrequency:execute(api)
    local param0
    
    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end
    
    for i, param in pairs(api.parameter_defs) do
        if (param.id == 0) then param0 = param.value end
    end

    if(self:verify_device(param0) == false) then
        api.result = "Device not found"
        return api
    end

    local result = GetDevice(param0):get_frequency()
    api = self:decode_result(api, result)

    return api
end

return GetFrequency
