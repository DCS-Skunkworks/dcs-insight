module("ServerSettings", package.seeall)

--- @class ServerSettings : APIBase
--- @field TCP_address string
--- @field TCP_port integer
local ServerSettings = {}

ServerSettings.TCP_address = "*"
ServerSettings.TCP_port = 7790

-- Log incoming and outgoing JSON
ServerSettings.Log_JSON = true

return ServerSettings
