module("Listener", package.seeall)

local TCPServer = require("Scripts.DCS-INSIGHT.lib.io.TCPServer")
local socket = require("socket") --[[@as Socket]]
local JSON = loadfile([[Scripts\JSON.lua]])()
local Log = require("Scripts.DCS-INSIGHT.lib.common.Log")
local ServerSettings = require("Scripts.DCS-INSIGHT.ServerSettings")

--- @class Listener
--- @field private host string the host to connect to
--- @field private port number the port on the host to connect to
--- @field public APIHandler APIHandler
--- @field public tcpServer TCPServer
--- @field public ReadClientData function
local Listener = {}

--- @func Returns new Listener
--- @return Listener
function Listener:new(host, port, APIHandler)
	local o = {
		host = host,
		port = port,
		APIHandler = APIHandler,
		tcpServer = {},
	}
	setmetatable(o, self)
	self.__index = self
	Listener.Instance = o -- holds the instance which can be called from the static function
	return o
end

--- Static read callback used by TCP Server
Listener.ReadClientData = function(str)
	Log:log_simple("Reading client request\n")

	if str == nil then
		return
	end

	if str == "SENDAPI" then
		local json = JSON:encode_pretty(Listener.Instance.APIHandler.apiTable)
		Listener.Instance.tcpServer:send(json)

		if ServerSettings.Log_JSON == true then
			Log:log_simple("Sending API list request\n")
			Log:log_simple("Outgoing JSON is\n" .. json)
		end
	else
		local command = JSON:decode(str)

		if ServerSettings.Log_JSON == true then
			local result_code, buffer = Log:dump_table(command, 100, 5000)
			Log:log_simple("Incoming JSON is\n" .. buffer)
		end

		local api = Listener.Instance.APIHandler:execute(command)
		local json = JSON:encode_pretty(api)
		Listener.Instance.tcpServer:send(json)

		if ServerSettings.Log_JSON == true then
			Log:log_simple("Sending API execution result for command id=" .. command.id .. "\n")
			Log:log_simple("Outgoing JSON is\n" .. json)
		end
	end
end

--- @func Initializes the TCPServer and APIHandler
function Listener:init()
	self.APIHandler:init()
	self.tcpServer = TCPServer:new(self.host, self.port, socket, self.ReadClientData)
end

return Listener
