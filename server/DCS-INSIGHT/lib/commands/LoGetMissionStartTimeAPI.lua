module("LoGetMissionStartTimeAPI", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 9

--- @class LoGetMissionStartTimeAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetMissionStartTimeAPI = APIBase:new()


--- @func Returns new LoGetMissionStartTimeAPI
function LoGetMissionStartTimeAPI:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "LoGetMissionStartTime()",
        0
    )

    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function LoGetMissionStartTimeAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetMissionStartTimeAPI:execute(api)
    
    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end

    local result = LoGetMissionStartTime()

    api = self:decode_result(api, result)

    return api
end

return LoGetMissionStartTimeAPI
