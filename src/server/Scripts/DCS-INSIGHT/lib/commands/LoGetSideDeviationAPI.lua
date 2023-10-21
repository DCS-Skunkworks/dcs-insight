module("LoGetSideDeviationAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

-- This is the unique ID for this particular API
local API_ID = 25

--- @class LoGetSideDeviationAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetSideDeviationAPI = APIBase:new()

--- @func Returns new LoGetSideDeviationAPI
function LoGetSideDeviationAPI:new(o)
	o = o or APIBase:new(o, API_ID, true, "LoGetSideDeviation()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetSideDeviationAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetSideDeviationAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetSideDeviation()

	api = self:decode_result(api, result)

	return api
end

return LoGetSideDeviationAPI
