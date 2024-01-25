module("LoGetNameByTypeInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoGetNameByTypeAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetNameByTypeAPI = APIBase:new()

--- Returns new LoGetNameByTypeAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetNameByTypeAPI:new(o, apiId)
	o = o
		or APIBase:new(o, apiId, true, "LoGetNameByType(weapon_level1, weapon_level2, weapon_level3, weapon_level4)", 4)

	o:add_param_def(0, ParamName.weapon_level1, ParamType.number)
	o:add_param_def(1, ParamName.weapon_level2, ParamType.number)
	o:add_param_def(2, ParamName.weapon_level3, ParamType.number)
	o:add_param_def(3, ParamName.weapon_level4, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGetNameByTypeAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetNameByTypeAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local param0
	local param1
	local param2
	local param3
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
		if param.id == 3 then
			param3 = param.value
		end
	end

	local result = LoGetNameByType(param0, param1, param2, param3)

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetNameByTypeAPI
