module("LoGetTrueAirSpeedInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoGetTrueAirSpeedAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetTrueAirSpeedAPI = APIBase:new()

--- Returns new LoGetTrueAirSpeedAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetTrueAirSpeedAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetTrueAirSpeed()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetTrueAirSpeedAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetTrueAirSpeedAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetTrueAirSpeed()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetTrueAirSpeedAPI
