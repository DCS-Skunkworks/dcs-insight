module("LoGetAltitudeAboveSeaLevelAPI", package.seeall)

local APIInfo = require("APIInfo")
local Parameter = require("Parameter")

-- This is the unique ID for this particular API
local API_ID = 18

--- @class LoGetAltitudeAboveSeaLevelAPI : APIHandlerBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetAltitudeAboveSeaLevelAPI = {}


--- @func Returns new LoGetAltitudeAboveSeaLevelAPI
function LoGetAltitudeAboveSeaLevelAPI:new()
    local apiInfo = APIInfo:new()
    apiInfo.id = API_ID
    apiInfo.returns_data = true
    apiInfo.api_syntax = "LoGetAltitudeAboveSeaLevel()"
    apiInfo.parameter_count = 0
    apiInfo.parameter_defs = {}


    --- @type LoGetAltitudeAboveSeaLevelAPI
    local o = {
        id = API_ID,
        apiInfo = apiInfo
    }
    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function LoGetAltitudeAboveSeaLevelAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetAltitudeAboveSeaLevelAPI:execute(api)

    api.result = LoGetAltitudeAboveSeaLevel()
    if (api.result == nil) then api.result = "result is nil" end

    if(type(api.result) == "table")then
        local result, str = Logg:dump_table(api.result, 100, 1000)        
        api.result = str
    end
    return api
end

return LoGetAltitudeAboveSeaLevelAPI
