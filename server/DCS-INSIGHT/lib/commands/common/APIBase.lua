module("APIBase", package.seeall)

local APIInfo = require("APIInfo")
local Parameter = require("Parameter")

--- @class APIBase
--- @field id number
--- @field returns_data boolean
--- @field api_syntax string
--- @field parameter_count number
--- @field private apiInfo APIInfo
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

--- @func Adds a parameter definition
--- @param id number
--- @param name string
--- @param type number
function APIBase:add_param_def(id, name, type)
	self.apiInfo.parameter_defs[#self.apiInfo.parameter_defs + 1] = Parameter:new(id, name, type)
end


--- @func Verifies that the parameter have values
--- @return number result_code, string error_message
function APIBase:verify_params()
    for i, param in pairs(self.apiInfo.parameter_defs) do
		if(param.value ~= nil) then
			return 1, "Parameter "..param.id.." value is nil"
		end
    end

	return 0, ""
end

--- @func Decodes result based on whether it is a function or procedure and sets api.result accordingly
--- @param api APIInfo
--- @param result any
function APIBase:decode_result(api, result)

	Logg:log("H1")
    if (result == nil) then 
		if(api.returns_data == true)then
			api.result = "result is nil"
		else
			api.result = "api was called"
		end
		Logg:log("H2")
		return api
	end
	
    if(type(result) == "table")then
		Logg:log("H3")
        local result, str = Logg:dump_table(result, 100, 2000)
        api.result = str
		return api
    end
	Logg:log("H4")
	api.result = result
    return api
end



return APIBase