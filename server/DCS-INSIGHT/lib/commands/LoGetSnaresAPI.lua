module("LoGetSnaresAPI", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 15

--- @class LoGetSnaresAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetSnaresAPI = APIBase:new()


--- @func Returns new LoGetSnaresAPI
function LoGetSnaresAPI:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "LoGetSnares()",
        0
    )

    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function LoGetSnaresAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetSnaresAPI:execute(api)
    
    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end

    local result = LoGetSnares()

    api = self:decode_result(api, result)

    return api
end

return LoGetSnaresAPI
