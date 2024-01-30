module("ServerSettingsDevelopment", package.seeall)

--- @class ServerSettingsDevelopment : APIBase
--- @field TCP_address string
--- @field TCP_port integer
local ServerSettingsDevelopment = {}

ServerSettingsDevelopment.TCP_address = "*"
ServerSettingsDevelopment.TCP_port = 7790

-- Log incoming and outgoing JSON
ServerSettingsDevelopment.Log_JSON = false

-- WARNING! This function enables arbitrary lua code to be
-- executed on your computer, including calls to package os (Operating System).
-- Do NOT enable unless your are firewalled.
ServerSettingsDevelopment.EnableLuaConsole = true

return ServerSettingsDevelopment
