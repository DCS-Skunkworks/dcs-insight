module("LoGetWingTargetsInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoGetWingTargetsAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetWingTargetsAPI = APIBase:new()

--- Returns new LoGetWingTargetsAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetWingTargetsAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetWingTargets()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetWingTargetsAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetWingTargetsAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetWingTargets()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetWingTargetsAPI
