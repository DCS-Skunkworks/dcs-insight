module("LoGetAccelerationUnitsAPI", package.seeall)

local APIInfo = require("APIInfo")
local Parameter = require("Parameter")

-- This is the unique ID for this particular API
local API_ID = 13

--- @class LoGetAccelerationUnitsAPI : APIHandlerBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetAccelerationUnitsAPI = {}


--- @func Returns new LoGetAccelerationUnitsAPI
function LoGetAccelerationUnitsAPI:new()
    local apiInfo = APIInfo:new()
    apiInfo.id = API_ID
    apiInfo.returns_data = true
    apiInfo.api_syntax = "LoGetAccelerationUnits()"
    apiInfo.parameter_count = 0
    apiInfo.parameter_defs = {}


    --- @type LoGetAccelerationUnitsAPI
    local o = {
        id = API_ID,
        apiInfo = apiInfo
    }
    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function LoGetAccelerationUnitsAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetAccelerationUnitsAPI:execute(api)

    api.result = LoGetAccelerationUnits()
    if (api.result == nil) then api.result = "result is nil" end

    if(type(api.result) == "table")then
        local result, str = Logg:dump_table(api.result, 100, 1000)        
        api.result = str
    end
    return api
end

return LoGetAccelerationUnitsAPI
