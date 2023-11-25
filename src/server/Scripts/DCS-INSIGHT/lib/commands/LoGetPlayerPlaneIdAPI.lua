module("LoGetPlayerPlaneIdAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoGetPlayerPlaneIdAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetPlayerPlaneIdAPI = APIBase:new()

--- @func Returns new LoGetPlayerPlaneIdAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetPlayerPlaneIdAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetPlayerPlaneId()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetPlayerPlaneIdAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetPlayerPlaneIdAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetPlayerPlaneId()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetPlayerPlaneIdAPI
