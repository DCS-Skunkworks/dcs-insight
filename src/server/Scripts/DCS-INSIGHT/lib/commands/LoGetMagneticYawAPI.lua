module("LoGetMagneticYawInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoGetMagneticYawAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetMagneticYawAPI = APIBase:new()

--- Returns new LoGetMagneticYawAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetMagneticYawAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetMagneticYaw()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetMagneticYawAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetMagneticYawAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetMagneticYaw()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetMagneticYawAPI
