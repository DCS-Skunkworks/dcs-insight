module("LoGetAltitudeAboveGroundLevelAPI", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 19

--- @class LoGetAltitudeAboveGroundLevelAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetAltitudeAboveGroundLevelAPI = APIBase:new()


--- @func Returns new LoGetAltitudeAboveGroundLevelAPI
function LoGetAltitudeAboveGroundLevelAPI:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "LoGetAltitudeAboveGroundLevel()",
        0
    )

    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function LoGetAltitudeAboveGroundLevelAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetAltitudeAboveGroundLevelAPI:execute(api)

    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end

    local result = LoGetAltitudeAboveGroundLevel()

    api = self:decode_result(api, result)

    return api
end

return LoGetAltitudeAboveGroundLevelAPI
