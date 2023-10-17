package.path = package.path .. ";.\\LuaSocket\\?.lua"
package.cpath = package.cpath .. ";.\\LuaSocket\\?.dll"

package.path = lfs.writedir() .. "?.lua;" .. package.path

local LogInsight = require("Scripts.DCS-INSIGHT.lib.common.LogInsight")
local ServerSettings = require("Scripts.DCS-INSIGHT.ServerSettings")
local APIHandler = require("Scripts.DCS-INSIGHT.lib.APIHandler")
local APIHandler = APIHandler:new()

local Listener = require("Scripts.DCS-INSIGHT.lib.Listener")

ListenerGlobal = Listener:new(ServerSettings.TCP_address, ServerSettings.TCP_port, APIHandler)
ListenerGlobal:init()

--local counter = 0
local function step(arg, time)
	if ListenerGlobal.tcpServer.step then
		ListenerGlobal.tcpServer:step()
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
	ListenerGlobal.tcpServer:init()

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportStart then
		PrevExport.LuaExportStart()
	end
end

LuaExportStop = function()
	ListenerGlobal.tcpServer:close()

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
			LogInsight:log_simple("Listener.step() failed: " .. err)
		end
		lastStepTime = currentTime + 0.1
	end

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportAfterNextFrame then
		PrevExport.LuaExportAfterNextFrame()
	end
end
