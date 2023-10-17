module("ServerSettings", package.seeall)

--- @class ServerSettings : APIBase
--- @field TCP_address string
--- @field TCP_port integer
local ServerSettings = {}

TCP_address = "*"
TCP_port = 7790

-- Log incoming and outgoing JSON
Log_JSON = false

return ServerSettings
