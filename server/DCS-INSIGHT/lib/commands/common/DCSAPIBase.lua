module("APIHandlerBase", package.seeall)

--- @class APIHandlerBase
local APIHandlerBase = {}

--- Constructs a new APIHandlerBase
function APIHandlerBase:new()
	--- @type APIHandlerBase
	local o = {}
	setmetatable(o, self)
	self.__index = self
	return o
end

--- @abstract
--- Creates the api
function APIHandlerBase:init()
	error("init must be implemented by the APIHandlerBase subclass", 2)
end

--- @abstract
--- Executes sent api and returns the same api containing a result field
--- @param api APIInfo
function APIHandlerBase:execute(api)
	error("step must be implemented by the APIHandlerBase subclass", 2)
end

return APIHandlerBase
