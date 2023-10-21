module("APIInfo", package.seeall)

local Parameter = require("Scripts.DCS-INSIGHT.lib.commands.common.Parameter")

--- @class APIInfo
--- @field id number
--- @field returns_data boolean
--- @field api_syntax string
--- @field parameter_count number
--- @field parameter_defs table<APIParameter>
--- @field error_thrown boolean
--- @field error_message string
--- @field result any
local APIInfo = {}

--- Constructs a new APIInfo
function APIInfo:new(id, returns_data, api_syntax, parameter_count, parameter_defs, error_thrown, error_message, result)
	--- @type APIInfo
	local o = {
		id = id,
		returns_data = returns_data,
		api_syntax = api_syntax,
		parameter_count = parameter_count,
		parameter_defs = parameter_defs or {},
		error_thrown = false,
		error_message = "",
		result = result,
	}
	setmetatable(o, self)
	self.__index = self
	return o
end

return APIInfo
