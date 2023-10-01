module("LoGetAccelerationUnitsAPI", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 15

--- @class LoGetAccelerationUnitsAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetAccelerationUnitsAPI = APIBase:new()


--- @func Returns new LoGetAccelerationUnitsAPI
function LoGetAccelerationUnitsAPI:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "LoGetAccelerationUnits()",
        0
    )

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

    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end

    local result = LoGetAccelerationUnits()

    api = self:decode_result(api, result)

    return api
end

return LoGetAccelerationUnitsAPI
