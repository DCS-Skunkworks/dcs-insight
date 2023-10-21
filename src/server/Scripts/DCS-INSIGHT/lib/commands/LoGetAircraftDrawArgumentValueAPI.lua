module("LoGetAircraftDrawArgumentValueAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

-- This is the unique ID for this particular API
local API_ID = 6

--- @class LoGetAircraftDrawArgumentValueAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetAircraftDrawArgumentValueAPI = APIBase:new()

--- @func Returns new LoGetAircraftDrawArgumentValueAPI
function LoGetAircraftDrawArgumentValueAPI:new(o)
	o = o or APIBase:new(o, API_ID, true, "LoGetAircraftDrawArgumentValue(draw_argument_id)", 1)

	o:add_param_def(0, ParamName.draw_argument_id, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetAircraftDrawArgumentValueAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetAircraftDrawArgumentValueAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local param0
	for i, param in pairs(api.parameter_defs) do
		if param.id == 0 then
			param0 = param.value
		end
	end

	local result = LoGetAircraftDrawArgumentValue(param0)

	api = self:decode_result(api, result)

	return api
end

return LoGetAircraftDrawArgumentValueAPI
