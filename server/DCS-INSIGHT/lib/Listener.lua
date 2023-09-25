module("Listener", package.seeall)

local TCPServer = require("TCPServer")
local socket = require("socket") --[[@as Socket]]
local JSON = loadfile([[Scripts\JSON.lua]])()

--- @class Listener
--- @field private host string the host to connect to
--- @field private port number the port on the host to connect to
--- @field public APIHandler APIHandler
--- @field public tcpServer TCPServer
local Listener = {}

--- @func Returns new Listener
--- @return Listener
function Listener:new(host, port, APIHandler)
	
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

Listener.ReadClientData = function (str)
	Logg:log_simple("Reading client request\n")

	if (str == nil) then return end
	
	if(str == "SENDAPI") then
		local json = JSON:encode_pretty(ListenerGlobal.APIHandler.apiTable)
		--Logg:log_simple("Outgoing JSON is\n"..json)
		ListenerGlobal.tcpServer:send(json)	
		Logg:log_simple("Sending API list request\n")
	else
		local command = JSON:decode(str);
		local result_code, buffer = Logg:dump_table(command, 100, 5000)
		--Logg:log_simple("Incoming JSON is\n"..buffer)
		local api = ListenerGlobal.APIHandler:execute(command)
		local json = JSON:encode_pretty(api)
		Logg:log_simple("Outgoing JSON is\n"..json)
		ListenerGlobal.tcpServer:send(json)
		Logg:log_simple("Sending API execution result for command id="..command.id.."\n")
	end
end

--- @func Initializes the TCPServer and APIHandler
function Listener:init()
	self.APIHandler:init()
	self.tcpServer = TCPServer:new(self.host, self.port, socket, self.ReadClientData)
end



return Listener
