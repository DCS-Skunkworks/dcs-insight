module("LoGetMissionStartTimeInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoGetMissionStartTimeAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetMissionStartTimeAPI = APIBase:new()

--- Returns new LoGetMissionStartTimeAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetMissionStartTimeAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetMissionStartTime()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetMissionStartTimeAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetMissionStartTimeAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetMissionStartTime()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetMissionStartTimeAPI
