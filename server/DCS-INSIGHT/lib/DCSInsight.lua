


package.path  = package.path..";.\\LuaSocket\\?.lua"
package.cpath = package.cpath..";.\\LuaSocket\\?.dll"

package.path = lfs.writedir() .. [[Scripts\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\io\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\commands\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\commands\common\?.lua;]] .. package.path
package.path = lfs.writedir() .. [[Scripts\DCS-INSIGHT\lib\global\?.lua;]] .. package.path

dofile(lfs.writedir()..[[Scripts\DCS-INSIGHT\server_settings.lua]])
dofile(lfs.writedir()..[[Scripts\DCS-INSIGHT\lib\global\enums.lua]])

local Logger = require("Logger")
Logg = Logger:new(lfs.writedir()..[[Logs\dcs-insight-server.log]])

local APIHandler = require("APIHandler")
local APIHandler = APIHandler:new()

local DCSConsole = require "DCSConsole"
DCSConsoleGlobal = DCSConsole:new(TCP_address, TCP_port, APIHandler)
DCSConsoleGlobal:init()

Logg:log_is_nil("DCSInsight: DCSConsoleGlobal.APIHandler", DCSConsoleGlobal.APIHandler)
Logg:log_is_nil("DCSInsight: DCSConsoleGlobal.APIHandler.apiTable", DCSConsoleGlobal.APIHandler.apiTable)
--Logg:log_table_indexes(DCSConsoleGlobal.APIHandler.apiTable)
--Logg:log_simple(DCSConsoleGlobal.APIHandler.apiTable[0])
Logg:log("DCSInsight: APIHandler.apiTable is ")
Logg:log_table(DCSConsoleGlobal.APIHandler.apiTable, 10, 100)
Logg:log("TABLE END \n\n")



local counter = 0;
local function step(arg, time)
	if(DCSConsoleGlobal.tcpServer.step) then
		DCSConsoleGlobal.tcpServer:step()
	end

	if(counter % 50 == 0) then
		--Logg:log_simple("STEP")
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
	DCSConsoleGlobal.tcpServer:init()

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportStart then
		PrevExport.LuaExportStart()
	end
end

LuaExportStop = function()
	DCSConsoleGlobal.tcpServer:close()

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
			Logg:log_simple("DCSConsole.step() failed: "..err)
		end
		lastStepTime = currentTime + .1
	end

	-- Chain previously-included export as necessary
	if PrevExport.LuaExportAfterNextFrame then
		PrevExport.LuaExportAfterNextFrame()
	end
end
