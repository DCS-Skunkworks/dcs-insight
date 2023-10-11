module("Listener", package.seeall)

local TCPServer = require("TCPServer")
local socket = require("socket") --[[@as Socket]]
local JSON = loadfile([[Scripts\JSON.lua]])()
local LogInsight = require("LogInsight")

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
	LogInsight:log_simple("Reading client request\n")

	if (str == nil) then return end
	
	if(str == "SENDAPI") then
		local json = JSON:encode_pretty(ListenerGlobal.APIHandler.apiTable)
		ListenerGlobal.tcpServer:send(json)	

		if(Log_JSON == true)then
			LogInsight:log_simple("Sending API list request\n")
			LogInsight:log_simple("Outgoing JSON is\n"..json)	
		end
	else
		local command = JSON:decode(str);

		if(Log_JSON == true)then			
			local result_code, buffer = LogInsight:dump_table(command, 100, 5000)
			LogInsight:log_simple("Incoming JSON is\n"..buffer)		
		end

		local api = ListenerGlobal.APIHandler:execute(command)
		local json = JSON:encode_pretty(api)
		ListenerGlobal.tcpServer:send(json)

		if(Log_JSON == true)then			
			LogInsight:log_simple("Sending API execution result for command id="..command.id.."\n")
			LogInsight:log_simple("Outgoing JSON is\n"..json)
		end
	end
end

--- @func Initializes the TCPServer and APIHandler
function Listener:init()
	self.APIHandler:init()
	self.tcpServer = TCPServer:new(self.host, self.port, socket, self.ReadClientData)
end



return Listener
