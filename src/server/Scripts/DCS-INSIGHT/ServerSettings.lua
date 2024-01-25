module("ServerSettings", package.seeall)

--- @class ServerSettings : APIBase
--- @field TCP_address string
--- @field TCP_port integer
local ServerSettings = {}

ServerSettings.TCP_address = "*"
ServerSettings.TCP_port = 7790

-- Log incoming and outgoing JSON
ServerSettings.Log_JSON = false

-- WARNING! This function enables arbitrary lua code to be
-- executed on your computer, including calls to package os (Operating System).
-- Do NOT enable unless your are firewalled.
ServerSettings.EnableLuaConsole = false

return ServerSettings
