module("LoGetAircraftDrawArgumentValueAPI", package.seeall)

local APIInfo = require("APIInfo")
local Parameter = require("Parameter")

-- This is the unique ID for this particular API
local API_ID = 6

--- @class LoGetAircraftDrawArgumentValueAPI : APIHandlerBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetAircraftDrawArgumentValueAPI = {}


--- @func Returns new LoGetAircraftDrawArgumentValueAPI
function LoGetAircraftDrawArgumentValueAPI:new()
    local apiInfo = APIInfo:new()
    apiInfo.id = API_ID
    apiInfo.returns_data = true
    apiInfo.api_syntax = "LoGetAircraftDrawArgumentValue(draw_argument_id)"
    apiInfo.parameter_count = 1
    apiInfo.parameter_defs = {}
    local parameter0 = Parameter:new(0, ParamName.draw_argument_id, ParamType.number)
    apiInfo.parameter_defs[#apiInfo.parameter_defs + 1] = parameter0


    --- @type LoGetAircraftDrawArgumentValueAPI
    local o = {
        id = API_ID,
        apiInfo = apiInfo
    }
    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function LoGetAircraftDrawArgumentValueAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetAircraftDrawArgumentValueAPI:execute(api)
    local param0

    for i, param in pairs(api.parameter_defs) do
        if (param.id == 0) then param0 = param.value end
    end

    if(param0 == nil) then 
        api.result = "Invalid parameter values." 
    else
        api.result = LoGetAircraftDrawArgumentValue(param0)
        if(api.result == nil) then api.result = "result is nil" end
    end

    return api
end

return LoGetAircraftDrawArgumentValueAPI
