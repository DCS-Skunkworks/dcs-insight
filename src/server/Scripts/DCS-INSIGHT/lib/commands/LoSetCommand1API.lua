module("LoSetCommand1API", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoSetCommand1API : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoSetCommand1API = APIBase:new()

--- @func Returns new LoSetCommand1API
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoSetCommand1API:new(o, apiId)
	o = o or APIBase:new(o, apiId, false, "LoSetCommand(iCommand)", 1)

	o:add_param_def(0, ParamName.iCommand, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoSetCommand1API:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoSetCommand1API:execute(api)
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

	local result = LoSetCommand(param0)

	api = self:decode_result(api, result)

	return api
end

return LoSetCommand1API
