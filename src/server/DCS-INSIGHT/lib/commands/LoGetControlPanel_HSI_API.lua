module("LoGetControlPanel_HSI_API", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

-- This is the unique ID for this particular API
local API_ID = 29

--- @class LoGetControlPanel_HSI_API : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetControlPanel_HSI_API = APIBase:new()

--- @func Returns new LoGetControlPanel_HSI_API
function LoGetControlPanel_HSI_API:new(o)
	o = o or APIBase:new(o, API_ID, true, "LoGetControlPanel_HSI()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetControlPanel_HSI_API:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetControlPanel_HSI_API:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.result = message
		return api
	end

	local result = LoGetControlPanel_HSI()

	api = self:decode_result(api, result)

	return api
end

return LoGetControlPanel_HSI_API
