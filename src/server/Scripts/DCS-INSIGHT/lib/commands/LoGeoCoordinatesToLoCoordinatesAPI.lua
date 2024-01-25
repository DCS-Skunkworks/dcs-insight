module("LoGeoCoordinatesToLoCoordinatesInsightAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoGeoCoordinatesToLoCoordinatesAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGeoCoordinatesToLoCoordinatesAPI = APIBase:new()

--- Returns new LoGeoCoordinatesToLoCoordinatesAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGeoCoordinatesToLoCoordinatesAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, false, "LoGeoCoordinatesToLoCoordinates(longitude_degrees, latitude_degrees)", 2)

	o:add_param_def(0, ParamName.longitude_degrees, ParamType.number)
	o:add_param_def(1, ParamName.latitude_degrees, ParamType.number)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- Inits with internal data
function LoGeoCoordinatesToLoCoordinatesAPI:init() end

--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGeoCoordinatesToLoCoordinatesAPI:execute(api)
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

	local x, z = LoGeoCoordinatesToLoCoordinates(param0, param1)
	local result = "x = " .. x .. " z = " .. z
	api = self:decode_result(api, result, "2 return values, all numbers")

	return api
end

return LoGeoCoordinatesToLoCoordinatesAPI
