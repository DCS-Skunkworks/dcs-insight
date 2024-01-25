module("LoGetControlPanel_HSI_InsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoGetControlPanel_HSI_API : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetControlPanel_HSI_API = APIBase:new()

--- Returns new LoGetControlPanel_HSI_API
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetControlPanel_HSI_API:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetControlPanel_HSI()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetControlPanel_HSI_API:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetControlPanel_HSI_API:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetControlPanel_HSI()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetControlPanel_HSI_API
