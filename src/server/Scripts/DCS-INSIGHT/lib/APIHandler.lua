module("APIHandler", package.seeall)

local Log = require("Scripts.DCS-INSIGHT.lib.common.Log")

local GetArgumentValueAPI = require("Scripts.DCS-INSIGHT.lib.commands.GetArgumentValueAPI")
local SetArgumentValueAPI = require("Scripts.DCS-INSIGHT.lib.commands.SetArgumentValueAPI")
local SetCommandAPI = require("Scripts.DCS-INSIGHT.lib.commands.SetCommandAPI")
local SetFrequencyAPI = require("Scripts.DCS-INSIGHT.lib.commands.SetFrequencyAPI")
local GetFrequencyAPI = require("Scripts.DCS-INSIGHT.lib.commands.GetFrequencyAPI")
local UpdateArgumentsAPI = require("Scripts.DCS-INSIGHT.lib.commands.UpdateArgumentsAPI")
local PerformClickableActionAPI = require("Scripts.DCS-INSIGHT.lib.commands.PerformClickableActionAPI")
local LoGetAircraftDrawArgumentValueAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAircraftDrawArgumentValueAPI")
local LoGetSelfDataAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetSelfDataAPI")
local LoGetModelTimeAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetModelTimeAPI")
local LoGetMissionStartTimeAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetMissionStartTimeAPI")
local LoGetPilotNameAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetPilotNameAPI")
local LoGetIndicatedAirSpeedAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetIndicatedAirSpeedAPI")
local LoGetAccelerationUnitsAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAccelerationUnitsAPI")
local LoGetADIPitchBankYawAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetADIPitchBankYawAPI")
local LoGetSnaresAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetSnaresAPI")
local ListIndicationAPI = require("Scripts.DCS-INSIGHT.lib.commands.ListIndicationAPI")
local ListCockpitParamsAPI = require("Scripts.DCS-INSIGHT.lib.commands.ListCockpitParamsAPI")
local LoGetAltitudeAboveSeaLevelAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAltitudeAboveSeaLevelAPI")
local LoGetAltitudeAboveGroundLevelAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAltitudeAboveGroundLevelAPI")
local LoGetVerticalVelocityAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetVerticalVelocityAPI")
local LoGetTrueAirSpeedAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetTrueAirSpeedAPI")
local LoGetMachNumberAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetMachNumberAPI")
local LoGetAngleOfAttackAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAngleOfAttackAPI")
local LoGetGlideDeviationAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetGlideDeviationAPI")
local LoGetSideDeviationAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetSideDeviationAPI")
local LoGetSlipBallPositionAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetSlipBallPositionAPI")
local LoGetEngineInfoAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetEngineInfoAPI")
local LoGetMechInfoAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetMechInfoAPI")
local LoGetControlPanel_HSI_API = require("Scripts.DCS-INSIGHT.lib.commands.LoGetControlPanel_HSI_API")
local LoGetPayloadInfoAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetPayloadInfoAPI")
local LoGetNavigationInfoAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetNavigationInfoAPI")
local LoGetMagneticYawAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetMagneticYawAPI")
local LoGetBasicAtmospherePressureAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetBasicAtmospherePressureAPI")
local LoGetMCPStateAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetMCPStateAPI")
local LoGetTWSInfoAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetTWSInfoAPI")
local LoGetAngleOfSideSlipAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAngleOfSideSlipAPI")
local LoGetRadarAltimeterAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetRadarAltimeterAPI")
local LoSetCommand1API = require("Scripts.DCS-INSIGHT.lib.commands.LoSetCommand1API")
local LoSetCommand2API = require("Scripts.DCS-INSIGHT.lib.commands.LoSetCommand2API")
local LoGetRouteAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetRouteAPI")
local LoGetWingInfoAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetWingInfoAPI")
local LoGetRadioBeaconsStatusAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetRadioBeaconsStatusAPI")
local LoGetVectorVelocityAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetVectorVelocityAPI")
local LoGetVectorWindVelocityAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetVectorWindVelocityAPI")
local LoGetAngularVelocityAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAngularVelocityAPI")
local LoGetFMDataAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetFMDataAPI")
local LoIsOwnshipExportAllowedAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoIsOwnshipExportAllowedAPI")
local LoIsObjectExportAllowedAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoIsObjectExportAllowedAPI")
local LoIsSensorExportAllowedAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoIsSensorExportAllowedAPI")
local LoGetObjectByIdAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetObjectByIdAPI")
local LoGetWorldObjectsAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetWorldObjectsAPI")
local LoGetTargetInformationAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetTargetInformationAPI")
local LoGetLockedTargetInformationAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetLockedTargetInformationAPI")
local LoGetF15_TWS_ContactsAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetF15_TWS_ContactsAPI")
local LoGetSightingSystemInfoAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetSightingSystemInfoAPI")
local LoGetWingTargetsAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetWingTargetsAPI")
local LoGeoCoordinatesToLoCoordinatesAPI =
	require("Scripts.DCS-INSIGHT.lib.commands.LoGeoCoordinatesToLoCoordinatesAPI")
local LoGetAltitudeAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoGetAltitudeAPI")
local LoCoordinatesToGeoCoordinatesAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoCoordinatesToGeoCoordinatesAPI")

--- @class APIHandler
--- @field public commandsTable table<APIBase>
--- @field public apiTable table<APIInfo>
local APIHandler = {}

--- @func Returns new APIHandler
function APIHandler:new()
	--- @type APIHandler
	local o = {
		commandsTable = {},
		apiTable = {},
	}
	setmetatable(o, self)
	self.__index = self
	return o
end

local id = 0
local function counter()
	id = id + 1
	return id
end

--- @func Fills the commands and api table with APIs having device_id as parameter
function APIHandler:addDeviceAPIs()
	self.commandsTable[#self.commandsTable + 1] = GetArgumentValueAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = SetArgumentValueAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = PerformClickableActionAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = SetCommandAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = GetFrequencyAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = SetFrequencyAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = UpdateArgumentsAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo
end

--- @func Fills the commands and api table with APIs taking parameters but not device_id
function APIHandler:addParameterAPIs()
	self.commandsTable[#self.commandsTable + 1] = LoGetAircraftDrawArgumentValueAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetObjectByIdAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = ListIndicationAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoSetCommand1API:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoSetCommand2API:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGeoCoordinatesToLoCoordinatesAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoCoordinatesToGeoCoordinatesAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo
end

--- @func Fills the commands and api table with APIs not taking parameters
function APIHandler:addParameterlessAPIs()
	self.commandsTable[#self.commandsTable + 1] = ListCockpitParamsAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetSelfDataAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetModelTimeAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetMissionStartTimeAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetPilotNameAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetIndicatedAirSpeedAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetAccelerationUnitsAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetADIPitchBankYawAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetSnaresAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetAltitudeAboveSeaLevelAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetAltitudeAboveGroundLevelAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetVerticalVelocityAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetTrueAirSpeedAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetMachNumberAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetAngleOfAttackAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetGlideDeviationAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetSideDeviationAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetSlipBallPositionAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetEngineInfoAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetMechInfoAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetControlPanel_HSI_API:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetPayloadInfoAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetNavigationInfoAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetMagneticYawAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetBasicAtmospherePressureAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetMCPStateAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetTWSInfoAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetAngleOfSideSlipAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetRadarAltimeterAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetRouteAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetWingInfoAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetRadioBeaconsStatusAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetVectorVelocityAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetVectorWindVelocityAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetAngularVelocityAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetFMDataAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetWorldObjectsAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetTargetInformationAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetLockedTargetInformationAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetF15_TWS_ContactsAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetSightingSystemInfoAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetWingTargetsAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoGetAltitudeAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	-- LoIs.. APIs

	self.commandsTable[#self.commandsTable + 1] = LoIsOwnshipExportAllowedAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoIsObjectExportAllowedAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo

	self.commandsTable[#self.commandsTable + 1] = LoIsSensorExportAllowedAPI:new(nil, counter())
	self.apiTable[#self.apiTable + 1] = self.commandsTable[#self.commandsTable].apiInfo
end

--- @func Fills the commands and api table
function APIHandler:init()
	self:addDeviceAPIs()
	self:addParameterAPIs()
	self:addParameterlessAPIs()
	self:verify_entries()
end

--- @func Executes the command and returns a command containing the result
--- @param api APIInfo
function APIHandler:execute(api)
	for k, v in pairs(self.commandsTable) do
		if api.id == v.id then
			local result_code, result = pcall(v.execute, v, api) -- = v:execute(api)
			if result_code == true then
				result.error_thrown = false
				result.error_message = ""
				return result
			else
				if result == nil then
					api.error_thrown = true
					api.error_message = "Error but no error message"
					api.result = nil
					return api
				else
					local path = v:script_path():gsub("%-", "%%-") -- escape any hyphen otherwise next gsub won't work
					--Log:log(result:gsub(path, ""))
					api.error_thrown = true
					api.result = nil
					api.error_message = result:gsub(path, "")
					return api
				end
			end
		end
	end
end

--- @func Performs checks on registered api
--- @return boolean
function APIHandler:verify_entries()
	local message = "Following api have been loaded :\n"
	for i = 1, #self.commandsTable do
		message = message .. self.commandsTable[i].id .. " : " .. self.commandsTable[i].apiInfo.api_syntax .. "\n"
	end
	Log:log(message)

	local seen = {}
	local duplicated = {}
	for i = 1, #self.commandsTable do
		local command = self.commandsTable[i]
		if seen[command.id] then --check if we've seen the element before
			duplicated[#duplicated + 1] = { id = command.id, api_syntax = command.apiInfo.api_syntax } --if we have then it must be a duplicate! add to a table to keep track of this
		else
			seen[command.id] = true -- set the element to seen
		end

		if command.apiInfo.parameter_count ~= #command.apiInfo.parameter_defs then
			Log:log("Parameter count mismatch in " .. command.apiInfo.api_syntax)
		end
	end

	local message = "Following api have duplicate id. This must be corrected :\n"
	local found = false
	for i = 1, #duplicated do
		message = message .. duplicated[i].id .. "  " .. duplicated[i].api_syntax .. "\n"
		found = true
	end

	if found then
		Log:log(message)
		error("dcs-insight API Id conflicts found")
	end

	return true
end

return APIHandler
