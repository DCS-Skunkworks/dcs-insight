module("LoGetPayloadInfoAPI", package.seeall)

local APIBase = require("Scripts.DCS-INSIGHT.lib.commands.common.APIBase")

-- This is the unique ID for this particular API
local API_ID = 30

--- @class LoGetPayloadInfoAPI : APIBase
--- @field id number API ID
--- @field apiInfo APIInfo
local LoGetPayloadInfoAPI = APIBase:new()

--- @func Returns new LoGetPayloadInfoAPI
function LoGetPayloadInfoAPI:new(o)
	o = o or APIBase:new(o, API_ID, true, "LoGetPayloadInfo()", 0)

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @func Inits with internal data
function LoGetPayloadInfoAPI:init() end

--- @func Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function LoGetPayloadInfoAPI:execute(api)
	local result_code, message = self:verify_params()
	if result_code == 1 then
		api.result = message
		return api
	end

	local result = LoGetPayloadInfo()

	api = self:decode_result(api, result)

	return api
end

return LoGetPayloadInfoAPI
