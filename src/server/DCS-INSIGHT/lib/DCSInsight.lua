


package.path  = package.path..";.\\LuaSocket\\?.lua"
package.cpath = package.cpath..";.\\LuaSocket\\?.dll"

package.path = lfs.writedir() .. [[Scripts\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\io\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\commands\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\commands\common\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\common\?.lua;]] .. package.path

dofile(lfs.writedir()..[[Scripts\DCS-INSIGHT\server_settings.lua]])
dofile(lfs.writedir()..[[Scripts\DCS-INSIGHT\lib\common\enums.lua]])

local LogInsight = require("LogInsight")
local APIHandler = require("APIHandler")
local APIHandler = APIHandler:new()

local Listener = require "Listener"
ListenerGlobal = Listener:new(TCP_address, TCP_port, APIHandler)
ListenerGlobal:init()


local counter = 0;
local function step(arg, time)
	if(ListenerGlobal.tcpServer.step) then
		ListenerGlobal.tcpServer:step()
	end

	if(counter % 50 == 0) then
		--Log:log_simple("STEP")
	end
	counter = counter + 1;
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
			LogInsight:log_simple("Listener.step() failed: "..err)
		end
		lastStepTime = currentTime + .1
	end

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportAfterNextFrame then
		PrevExport.LuaExportAfterNextFrame()
	end
end
