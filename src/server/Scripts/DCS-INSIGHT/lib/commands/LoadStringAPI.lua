module("LoadStringAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local Parameter = require("Scripts.DCS-INSIGHT.lib.commands.common.Parameter")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoadStringAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoadStringAPI = APIBase:new()

--- @func Returns new LoadStringAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoadStringAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, false, "LuaConsole", 1)

	o:add_param_def(0, ParamName.lua_code, ParamType.string)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoadStringAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoadStringAPI:execute(api)
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

	local f, err = loadstring(param0)
	if f then
		local result, err2 = pcall(f())
		if err2 then
			api.error_thrown = true
			api.error_message = err2
			return api
		end
		api = self:decode_result(api, result, nil)
		return api
	else
		api.error_thrown = true
		if err ~= nil then
			api.error_message = err
			return api
		end
		api.error_message = "Error parsing lua snippet."
		return api
	end
end

return LoadStringAPI
