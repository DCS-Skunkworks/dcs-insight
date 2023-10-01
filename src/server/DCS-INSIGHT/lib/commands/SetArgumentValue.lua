module("SetArgumentValue", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 1

--- @class SetArgumentValue : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local SetArgumentValue = APIBase:new()


--- @func Returns new SetArgumentValue
function SetArgumentValue:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        false,
        "GetDevice(device_id):set_argument_value(argument_id, new_value)",
        3
    )

    o:add_param_def(0, ParamName.device_id, ParamType.number)
    o:add_param_def(1, ParamName.argument_id, ParamType.number)
    o:add_param_def(2, ParamName.new_value, ParamType.number)

    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function SetArgumentValue:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function SetArgumentValue:execute(api)
    local param0
    local param1
    local param2

    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end

    for i, param in pairs(api.parameter_defs) do
        if (param.id == 0) then param0 = param.value end
        if (param.id == 1) then param1 = param.value end
        if (param.id == 2) then param2 = param.value end
    end

    if(self:verify_device(param0) == false) then
        api.result = "Device not found"
        return api
    end

    local result = GetDevice(param0):set_argument_value(param1, param2)        
    
    api = self:decode_result(api, result)

    return api
end

return SetArgumentValue
