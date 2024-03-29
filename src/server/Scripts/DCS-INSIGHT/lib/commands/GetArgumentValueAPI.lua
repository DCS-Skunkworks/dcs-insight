module("GetArgumentValueInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class GetArgumentValueAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local GetArgumentValueAPI = APIBase:new()

--- Returns new GetArgumentValueAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function GetArgumentValueAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "GetDevice(device_id):get_argument_value(argument_id)", 2)

	o:add_param_def(0, ParamName.device_id, ParamType.number)
	o:add_param_def(1, ParamName.argument_id, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function GetArgumentValueAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function GetArgumentValueAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local param0, param1
	for i, param in pairs(api.parameter_defs) do
		if param.id == 0 then
			param0 = param.value
		end
		if param.id == 1 then
			param1 = param.value
		end
	end

	if self:verify_device(param0) == false then
		api.error_thrown = true
		api.error_message = "Device not found"
		return api
	end

	local result = GetDevice(param0):get_argument_value(param1)

	api = self:decode_result(api, result, nil)

	return api
end

return GetArgumentValueAPI
