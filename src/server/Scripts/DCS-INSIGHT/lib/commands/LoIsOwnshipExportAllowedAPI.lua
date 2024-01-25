module("LoIsOwnshipExportAllowedInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoIsOwnshipExportAllowedAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoIsOwnshipExportAllowedAPI = APIBase:new()

--- Returns new LoIsOwnshipExportAllowedAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoIsOwnshipExportAllowedAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoIsOwnshipExportAllowed()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoIsOwnshipExportAllowedAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoIsOwnshipExportAllowedAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoIsOwnshipExportAllowed()

	api = self:decode_result(api, result, nil)

	return api
end

return LoIsOwnshipExportAllowedAPI
