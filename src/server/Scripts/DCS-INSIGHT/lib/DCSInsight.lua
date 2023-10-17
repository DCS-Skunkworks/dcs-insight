--[[
   ____  _         _             _          _      _              _            __                    _         _      _            
  / ___|| |  ___  | |__    __ _ | |   ___  | |__  (_)  ___   ___ | |_  ___    / /__   __ __ _  _ __ (_)  __ _ | |__  | |  ___  ___ 
 | |  _ | | / _ \ | '_ \  / _` || |  / _ \ | '_ \ | | / _ \ / __|| __|/ __|  / / \ \ / // _` || '__|| | / _` || '_ \ | | / _ \/ __|
 | |_| || || (_) || |_) || (_| || | | (_) || |_) || ||  __/| (__ | |_ \__ \ / /   \ V /| (_| || |   | || (_| || |_) || ||  __/\__ \
  \____||_| \___/ |_.___  \__,_||_|  \___/ |_.__/_/ |_\___| ____| \__||___//_/     \_/  \__,_||_|   |_| \__,_||_.__/ |_| \___||___/
   __ _  _ __  ___   / _|  ___   _ __ | |__  (_)|___| |  __| |  ___  _ __                                                          
  / _` || '__|/ _ \ | |_  / _ \ | '__|| '_ \ | | / _` | / _` | / _ \| '_ \                                                         
 | (_| || |  |  __/ |  _|| (_) || |   | |_) || || (_| || (_| ||  __/| | | |                                                        
  \__,_||_|   \___| |_|   \___/ |_|   |_.__/ |_| \__,_| \__,_| \___||_| |_|                                                        
]]

package.path = package.path .. ";.\\LuaSocket\\?.lua"
package.cpath = package.cpath .. ";.\\LuaSocket\\?.dll"

package.path = lfs.writedir() .. "?.lua;" .. package.path

local Log = require("Scripts.DCS-INSIGHT.lib.common.Log")
local ServerSettings = require("Scripts.DCS-INSIGHT.ServerSettings")
local APIHandler = require("Scripts.DCS-INSIGHT.lib.APIHandler")
local APIHandler = APIHandler:new()

local Listener = require("Scripts.DCS-INSIGHT.lib.Listener")

local Listener = Listener:new(ServerSettings.TCP_address, ServerSettings.TCP_port, APIHandler)
Listener:init()

--local counter = 0
local function step(arg, time)
	if Listener.tcpServer.step then
		Listener.tcpServer:step()
	end

	--[[if counter % 50 == 0 then
		Log:log_simple("STEP")
	end
	counter = counter + 1]]
end

-- Prev Export functions.
local PrevExport = {}
PrevExport.LuaExportStart = LuaExportStart
PrevExport.LuaExportStop = LuaExportStop
PrevExport.LuaExportBeforeNextFrame = LuaExportBeforeNextFrame
PrevExport.LuaExportAfterNextFrame = LuaExportAfterNextFrame

local lastStepTime = 0

-- Lua Export Functions
LuaExportStart = function()
	Listener.tcpServer:init()

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportStart then
		PrevExport.LuaExportStart()
	end
end

LuaExportStop = function()
	Listener.tcpServer:close()

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportStop then
		PrevExport.LuaExportStop()
	end
end

function LuaExportBeforeNextFrame()
	-- Chain previously-included export as necessary
	if PrevExport.LuaExportBeforeNextFrame then
		PrevExport.LuaExportBeforeNextFrame()
	end
end

function LuaExportAfterNextFrame()
	local currentTime = LoGetModelTime()

	if currentTime >= lastStepTime then
		local bool, err = pcall(step)
		err = err or "something failed"
		if not bool then
			Log:log_simple("Listener.step() failed: " .. err)
		end
		lastStepTime = currentTime + 0.1
	end

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportAfterNextFrame then
		PrevExport.LuaExportAfterNextFrame()
	end
end
