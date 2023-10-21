module("LoGetADIPitchBankYawAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

-- This is the unique ID for this particular API
local API_ID = 16

--- @class LoGetADIPitchBankYawAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetADIPitchBankYawAPI = APIBase:new()

--- @func Returns new LoGetADIPitchBankYawAPI
function LoGetADIPitchBankYawAPI:new(o)
	o = o or APIBase:new(o, API_ID, true, "LoGetADIPitchBankYaw()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetADIPitchBankYawAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
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
