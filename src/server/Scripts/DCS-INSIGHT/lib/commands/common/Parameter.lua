module("APIParameter", package.seeall)

--- @class APIParameter
--- @field id number
--- @field name string
--- @field type number
--- @field value string
local APIParameter = {}

--- Returns new APIParameter
--- @param id number
--- @param name string
--- @param type integer
--- @param value any
function APIParameter:new(id, name, type, value)
	local o = {
		id = id,
		name = name,
		type = type,
		value = value,
	}
	setmetatable(o, self)
	self.__index = self
	return o
end

return APIParameter
