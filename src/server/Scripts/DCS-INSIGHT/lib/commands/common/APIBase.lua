module("APIBase", package.seeall)

local APIInfo = require("Scripts.DCS-INSIGHT.lib.commands.common.APIInfo")
local Parameter = require("Scripts.DCS-INSIGHT.lib.commands.common.Parameter")
local Log = require("Scripts.DCS-INSIGHT.lib.common.Log")

--- @class APIBase
--- @field id number
--- @field returns_data boolean
--- @field api_syntax string
--- @field parameter_count number
--- @field apiInfo APIInfo
local APIBase = {}

--- Constructs a new APIBase
function APIBase:new(o, id, returns_data, api_syntax, parameter_count)
	--- @type APIBase
	o = o or {}
	local apiInfo = APIInfo:new(id, returns_data, api_syntax, parameter_count)

	o.id = id
	o.parameter_count = parameter_count
	o.apiInfo = apiInfo

	setmetatable(o, self)
	self.__index = self
	return o
end

--- @abstract
--- Creates the api
function APIBase:init()
	error("init must be implemented by the APIBase subclass", 2)
end

--- @abstract
--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function APIBase:execute(api)
	error("step must be implemented by the APIBase subclass", 2)
end

--- Adds a parameter definition
--- @param id number
--- @param name string
--- @param type number
function APIBase:add_param_def(id, name, type)
	self.apiInfo.parameter_defs[#self.apiInfo.parameter_defs + 1] = Parameter:new(id, name, type)
end

--- Verifies that the parameter have values
--- @return number result_code, string error_message
function APIBase:verify_params()
	for i, param in pairs(self.apiInfo.parameter_defs) do
		if param.value ~= nil then
			return 1, "Parameter " .. param.id .. " value is nil"
		end
	end

	return 0, ""
end

--- Decodes result based on whether it is a function or procedure and sets api.result accordingly
--- @param api APIInfo
--- @param result any
--- @param result_type string|nil
function APIBase:decode_result(api, result, result_type)
	if result == nil then
		if api.returns_data == true then
			api.result = "result is nil"
		else
			api.result = "api was called"
		end
		return api
	end

	if result_type == nil then
		api.result_type = type(result)
	else
		api.result_type = result_type
	end

	if type(result) == "table" then
		local result, str = Log:dump_table(result, 100, 2000)
		if string.len(str) == 0 then
			api.result = "returned empty table"
			return api
		end
		api.result = str
		return api
	end

	api.result = result
	return api
end

--- Checks that there is a device with that number
--- @param device_id number
--- @return boolean
function APIBase:verify_device(device_id)
	local device = GetDevice(device_id)
	if device == nil then
		return false
	else
		return true
	end
end

--- Returns the path of the caller (file system)
--- @return string
function APIBase:script_path()
	local str = debug.getinfo(2, "S").source:sub(2)
	return str:match("(.*[/\\])") or ""
end

return APIBase
