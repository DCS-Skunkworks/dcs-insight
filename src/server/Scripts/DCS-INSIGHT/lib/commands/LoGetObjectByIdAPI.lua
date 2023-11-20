module("LoGetObjectByIdAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoGetObjectByIdAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetObjectByIdAPI = APIBase:new()

--- @func Returns new LoGetObjectByIdAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetObjectByIdAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetObjectById(object_id)", 1)

	o:add_param_def(0, ParamName.object_id, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetObjectByIdAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetObjectByIdAPI:execute(api)
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

	local result = LoGetObjectById(param0)

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetObjectByIdAPI
