module("ListIndicationAPI", package.seeall)

local APIBase = require("APIBase")

-- This is the unique ID for this particular API
local API_ID = 7

--- @class ListIndicationAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local ListIndicationAPI = APIBase:new()


--- @func Returns new ListIndicationAPI
function ListIndicationAPI:new(o)
    o = o or APIBase:new(
        o,
        API_ID,
        true,
        "list_indication(indicator_id)",
        1
    )
    
    o:add_param_def(0, ParamName.indicator_id, ParamType.number)
    
    setmetatable(o, self)
    self.__index = self
    return o
end

--- @func Inits with internal data
function ListIndicationAPI:init()
end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function ListIndicationAPI:execute(api)

    local result_code, message = self:verify_params()
    if(result_code == 1)then
        api.result = message
        return api
    end

    local param0
    for i, param in pairs(api.parameter_defs) do
        if (param.id == 0) then param0 = param.value end
    end

    local result = list_indication(param0)
    
    api = self:decode_result(api, result)

    return api
end

return ListIndicationAPI
