module("GetArgumentValue", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 0

--- @class GetArgumentValue : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local GetArgumentValue = APIBase:new()


--- @func Returns new GetArgumentValue
--- @return GetArgumentValue
function GetArgumentValue:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "GetDevice(device_id):get_argument_value(argument_id)",
        2
    )

    o:add_param_def(0, ParamName.device_id, ParamType.number)
    o:add_param_def(1, ParamName.argument_id, ParamType.number)

    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function GetArgumentValue:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function GetArgumentValue:execute(api)

    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end

    local param0, param1
    for i, param in pairs(api.parameter_defs) do
        if (param.id == 0) then param0 = param.value end
        if (param.id == 1) then param1 = param.value end
    end

    local result = GetDevice(param0):get_argument_value(param1)
    
    api = self:decode_result(api, result)

    return api
end

return GetArgumentValue
