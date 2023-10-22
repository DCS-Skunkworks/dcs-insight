module("LoGetGlideDeviationAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

--- @class LoGetGlideDeviationAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetGlideDeviationAPI = APIBase:new()

--- @func Returns new LoGetGlideDeviationAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetGlideDeviationAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetGlideDeviation()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetGlideDeviationAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetGlideDeviationAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetGlideDeviation()

	api = self:decode_result(api, result)

	return api
end

return LoGetGlideDeviationAPI
