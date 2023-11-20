module("ListCockpitParamsAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class ListCockpitParamsAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local ListCockpitParamsAPI = APIBase:new()

--- @func Returns new ListCockpitParamsAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function ListCockpitParamsAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "list_cockpit_params()", 0)

	setmetatable(o, self)
	self.__index = self

	return o
end

--- @func Inits with internal data
function ListCockpitParamsAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function ListCockpitParamsAPI:execute(api)
	local result = list_cockpit_params()

	api = self:decode_result(api, result, nil)

	return api
end

return ListCockpitParamsAPI
