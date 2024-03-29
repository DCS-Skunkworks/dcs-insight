module("LoSetCommand2InsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoSetCommand2API : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoSetCommand2API = APIBase:new()

--- Returns new LoSetCommand2API
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoSetCommand2API:new(o, apiId)
	o = o or APIBase:new(o, apiId, false, "LoSetCommand(iCommand, new_value)", 2)

	o:add_param_def(0, ParamName.iCommand, ParamType.number)
	o:add_param_def(1, ParamName.new_value, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoSetCommand2API:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoSetCommand2API:execute(api)
	local param0
	local param1

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
	end

	local result = LoSetCommand(param0, param1)

	api = self:decode_result(api, result, nil)

	return api
end

return LoSetCommand2API
