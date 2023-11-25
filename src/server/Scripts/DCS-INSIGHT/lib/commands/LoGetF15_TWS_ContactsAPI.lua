module("LoGetF15_TWS_ContactsAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")
local ParamName = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamName")
local ParamType = require("Scripts.DCS-INSIGHT.lib.commands.common.ParamType")

--- @class LoGetF15_TWS_ContactsAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetF15_TWS_ContactsAPI = APIBase:new()

--- @func Returns new LoGetF15_TWS_ContactsAPI
--- @param o table|nil Parent
--- @param apiId integer API ID, must be unique
--- @return APIBase
function LoGetF15_TWS_ContactsAPI:new(o, apiId)
	o = o or APIBase:new(o, apiId, true, "LoGetF15_TWS_Contacts()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetF15_TWS_ContactsAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetF15_TWS_ContactsAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.error_thrown = true
		api.error_message = message
		return api
	end

	local result = LoGetF15_TWS_Contacts()

	api = self:decode_result(api, result, nil)

	return api
end

return LoGetF15_TWS_ContactsAPI
