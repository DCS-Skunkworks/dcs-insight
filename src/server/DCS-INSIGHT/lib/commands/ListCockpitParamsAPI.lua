module("ListCockpitParamsAPI", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 8

--- @class ListCockpitParamsAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local ListCockpitParamsAPI = APIBase:new()


--- @func Returns new ListCockpitParamsAPI
function ListCockpitParamsAPI:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "list_cockpit_params()",
        0
    )

    setmetatable(o, self)
    self.__index = self
    
    return o
end

--- @func Inits with internal data
function ListCockpitParamsAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function ListCockpitParamsAPI:execute(api)

    local result = list_cockpit_params()
    
    api = self:decode_result(api, result)

    return api
end

return ListCockpitParamsAPI
