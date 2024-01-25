module("LoGetADIPitchBankYawInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoGetADIPitchBankYawAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetADIPitchBankYawAPI = APIBase:new()

--- Returns new LoGetADIPitchBankYawAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetADIPitchBankYawAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetADIPitchBankYaw()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetADIPitchBankYawAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetADIPitchBankYawAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local pitch, bank, yaw = LoGetADIPitchBankYaw()
	api.result = "pitch=" .. pitch .. ", bank=" .. bank .. ", yaw=" .. yaw

	return api
end

return LoGetADIPitchBankYawAPI
