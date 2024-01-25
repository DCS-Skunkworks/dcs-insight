module("LoGetWingInfoInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoGetWingInfoAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetWingInfoAPI = APIBase:new()

--- Returns new LoGetWingInfoAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetWingInfoAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetWingInfo()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetWingInfoAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetWingInfoAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetWingInfo()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetWingInfoAPI
