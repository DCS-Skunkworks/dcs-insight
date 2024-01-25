module("SetCommandInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class SetCommandAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local SetCommandAPI = APIBase:new()

--- Returns new SetCommandAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function SetCommandAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, false, "GetDevice(device_id):SetCommand(command_id, new_value)", 3)

	o:add_param_def(0, ParamName.device_id, ParamType.number)
	o:add_param_def(1, ParamName.command_id, ParamType.number)
	o:add_param_def(2, ParamName.new_value, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function SetCommandAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function SetCommandAPI:execute(api)
	local param0
	local param1
	local param2

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
		if param.id == 1 then
			param1 = param.value
		end
		if param.id == 2 then
			param2 = param.value
		end
	end

	if self:verify_device(param0) == false then
		api.error_thrown = true
		api.error_message = "Device not found"
		return api
	end

	local result = GetDevice(param0):SetCommand(param1, param2)
	api = self:decode_result(api, result, nil)

	return api
end

return SetCommandAPI
