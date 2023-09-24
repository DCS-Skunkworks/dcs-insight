module("DCSConsole", package.seeall)

local TCPServer = require("TCPServer")
local socket = require("socket") --[[@as Socket]]
local JSON = loadfile([[Scripts\JSON.lua]])()

--- @class DCSConsole
--- @field private host string the host to connect to
--- @field private port number the port on the host to connect to
--- @field public APIHandler APIHandler
--- @field public tcpServer TCPServer
local DCSConsole = {}

--- @func Returns new DCSConsole
--- @return DCSConsole
function DCSConsole:new(host, port, APIHandler)
	
	local o = {
		host = host,
		port = port,
		APIHandler = APIHandler,
		tcpServer = {}
	}
	setmetatable(o, self)
	self.__index = self
	return o
end

DCSConsole.ReadClientData = function (str)
	Logg:log_simple("ReadClientData "..str.."\n")

	if (str == nil) then return end
	
	if(str == "SENDAPI") then
		local json = JSON:encode_pretty(DCSConsoleGlobal.APIHandler.apiTable)
		Logg:log_simple("Outgoing JSON is\n"..json)
		DCSConsoleGlobal.tcpServer:send(json)			
	else
		JSON.onDecodeError = JSONError
		local command = JSON:decode(str);
		--local result_code, buffer = Logg:dump_table(command, 100, 5000)
		local api = DCSConsoleGlobal.APIHandler:execute(command)		
		local json = JSON:encode_pretty(api)
		Logg:log_simple("Outgoing JSON is\n"..json)
		DCSConsoleGlobal.tcpServer:send(json)
	end
end

function JSONError(message, text, location)
	Logg:log_simple("JSON ERROR")
	Logg:log_simple(message)
	Logg:log_simple(text)
	Logg:log_simple(location)
end

--- @func Initializes the TCPServer and APIHandler
function DCSConsole:init()
	self.APIHandler:init()
	self.tcpServer = TCPServer:new(self.host, self.port, socket, self.ReadClientData)
end



return DCSConsole
