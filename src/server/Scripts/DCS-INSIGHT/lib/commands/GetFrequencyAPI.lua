module("GetFrequencyAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class GetFrequencyAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local GetFrequencyAPI = APIBase:new()

--- @func Returns new GetFrequencyAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function GetFrequencyAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "GetDevice(device_id):get_frequency()", 1)

	o:add_param_def(0, ParamName.device_id, ParamType.number)

	setmetatable(o, self)
	self.__index = self

	return o
end

--- @func Inits with internal data
function GetFrequencyAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function GetFrequencyAPI:execute(api)
	local param0

	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	for i, param in pairs(api.parameter_defs) do
		if param.id == 0 then
			param0 = param.value
		end
	end

	if self:verify_device(param0) == false then
		api.error_thrown = true
		api.error_message = "Device not found"
		return api
	end

	local result = GetDevice(param0):get_frequency()
	api = self:decode_result(api, result)

	return api
end

return GetFrequencyAPI
