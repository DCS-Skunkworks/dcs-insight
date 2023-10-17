module("SetFrequency", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local Parameter = require("Scripts.DCS-INSIGHT.lib.commands.common.Parameter")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

-- This is the unique ID for this particular API
local API_ID = 5

--- @class SetFrequency : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local SetFrequency = APIBase:new()

--- @func Returns new SetFrequency
function SetFrequency:new(o)
	o = o or APIBase:new(o, API_ID, false, "GetDevice(device_id):set_frequency(new_value)", 2)

	o:add_param_def(0, ParamName.device_id, ParamType.number)
	o:add_param_def(1, ParamName.new_value, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function SetFrequency:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function SetFrequency:execute(api)
	local param0
	local param1

	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.result = message
		return api
	end

	for i, param in pairs(api.parameter_defs) do
		if param.id == 0 then
			param0 = param.value
		end
		if param.id == 1 then
			param1 = param.value
		end
	end

	if self:verify_device(param0) == false then
		api.result = "Device not found"
		return api
	end

	local result = GetDevice(param0):set_frequency(param1)
	api = self:decode_result(api, result)

	return api
end

return SetFrequency
