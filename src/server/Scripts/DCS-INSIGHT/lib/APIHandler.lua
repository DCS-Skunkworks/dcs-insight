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
local LoIsOwnshipExportAllowedAPI = require("Scripts.DCS-INSIGHT.lib.commands.LoIsOwnshipExportAllowedAPI")
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

--- @func Fills the commands and api table
function APIHandler:init()
	local getArgumentValueAPI = GetArgumentValueAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = getArgumentValueAPI
	self.apiTable[#self.apiTable + 1] = getArgumentValueAPI.apiInfo

	local setArgumentValueAPI = SetArgumentValueAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = setArgumentValueAPI
	self.apiTable[#self.apiTable + 1] = setArgumentValueAPI.apiInfo

	local performClickableActionAPI = PerformClickableActionAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = performClickableActionAPI
	self.apiTable[#self.apiTable + 1] = performClickableActionAPI.apiInfo

	local setCommandAPI = SetCommandAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = setCommandAPI
	self.apiTable[#self.apiTable + 1] = setCommandAPI.apiInfo

	local getFrequencyAPI = GetFrequencyAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = getFrequencyAPI
	self.apiTable[#self.apiTable + 1] = getFrequencyAPI.apiInfo

	local setFrequencyAPI = SetFrequencyAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = setFrequencyAPI
	self.apiTable[#self.apiTable + 1] = setFrequencyAPI.apiInfo

	local updateArgumentsAPI = UpdateArgumentsAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = updateArgumentsAPI
	self.apiTable[#self.apiTable + 1] = updateArgumentsAPI.apiInfo

	--[[ APIs not requiring device parameter below ]]

	local loGetAircraftDrawArgumentValue = LoGetAircraftDrawArgumentValueAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetAircraftDrawArgumentValue
	self.apiTable[#self.apiTable + 1] = loGetAircraftDrawArgumentValue.apiInfo

	local listIndicationAPI = ListIndicationAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = listIndicationAPI
	self.apiTable[#self.apiTable + 1] = listIndicationAPI.apiInfo

	local loSetCommand1API = LoSetCommand1API:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loSetCommand1API
	self.apiTable[#self.apiTable + 1] = loSetCommand1API.apiInfo

	local loSetCommand2API = LoSetCommand2API:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loSetCommand2API
	self.apiTable[#self.apiTable + 1] = loSetCommand2API.apiInfo

	--[[ APIs not requiring parameters below ]]

	local listCockpitParamsAPI = ListCockpitParamsAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = listCockpitParamsAPI
	self.apiTable[#self.apiTable + 1] = listCockpitParamsAPI.apiInfo

	local loGetSelfData = LoGetSelfDataAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetSelfData
	self.apiTable[#self.apiTable + 1] = loGetSelfData.apiInfo

	local loGetModelTimeAPI = LoGetModelTimeAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetModelTimeAPI
	self.apiTable[#self.apiTable + 1] = loGetModelTimeAPI.apiInfo

	local loGetMissionStartTimeAPI = LoGetMissionStartTimeAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetMissionStartTimeAPI
	self.apiTable[#self.apiTable + 1] = loGetMissionStartTimeAPI.apiInfo

	local loIsOwnshipExportAllowedAPI = LoIsOwnshipExportAllowedAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loIsOwnshipExportAllowedAPI
	self.apiTable[#self.apiTable + 1] = loIsOwnshipExportAllowedAPI.apiInfo

	local loGetPilotName = LoGetPilotNameAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetPilotName
	self.apiTable[#self.apiTable + 1] = loGetPilotName.apiInfo

	local loGetIndicatedAirSpeedAPI = LoGetIndicatedAirSpeedAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetIndicatedAirSpeedAPI
	self.apiTable[#self.apiTable + 1] = loGetIndicatedAirSpeedAPI.apiInfo

	local loGetAccelerationUnitsAPI = LoGetAccelerationUnitsAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetAccelerationUnitsAPI
	self.apiTable[#self.apiTable + 1] = loGetAccelerationUnitsAPI.apiInfo

	local loGetADIPitchBankYawAPI = LoGetADIPitchBankYawAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetADIPitchBankYawAPI
	self.apiTable[#self.apiTable + 1] = loGetADIPitchBankYawAPI.apiInfo

	local loGetSnaresAPI = LoGetSnaresAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetSnaresAPI
	self.apiTable[#self.apiTable + 1] = loGetSnaresAPI.apiInfo

	local loGetAltitudeAboveSeaLevelAPI = LoGetAltitudeAboveSeaLevelAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetAltitudeAboveSeaLevelAPI
	self.apiTable[#self.apiTable + 1] = loGetAltitudeAboveSeaLevelAPI.apiInfo

	local loGetAltitudeAboveGroundLevelAPI = LoGetAltitudeAboveGroundLevelAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetAltitudeAboveGroundLevelAPI
	self.apiTable[#self.apiTable + 1] = loGetAltitudeAboveGroundLevelAPI.apiInfo

	local loGetVerticalVelocityAPI = LoGetVerticalVelocityAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetVerticalVelocityAPI
	self.apiTable[#self.apiTable + 1] = loGetVerticalVelocityAPI.apiInfo

	local loGetTrueAirSpeedAPI = LoGetTrueAirSpeedAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetTrueAirSpeedAPI
	self.apiTable[#self.apiTable + 1] = loGetTrueAirSpeedAPI.apiInfo

	local loGetMachNumberAPI = LoGetMachNumberAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetMachNumberAPI
	self.apiTable[#self.apiTable + 1] = loGetMachNumberAPI.apiInfo

	local loGetAngleOfAttackAPI = LoGetAngleOfAttackAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetAngleOfAttackAPI
	self.apiTable[#self.apiTable + 1] = loGetAngleOfAttackAPI.apiInfo

	local loGetGlideDeviationAPI = LoGetGlideDeviationAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetGlideDeviationAPI
	self.apiTable[#self.apiTable + 1] = loGetGlideDeviationAPI.apiInfo

	local loGetSideDeviationAPI = LoGetSideDeviationAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetSideDeviationAPI
	self.apiTable[#self.apiTable + 1] = loGetSideDeviationAPI.apiInfo

	local loGetSlipBallPositionAPI = LoGetSlipBallPositionAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetSlipBallPositionAPI
	self.apiTable[#self.apiTable + 1] = loGetSlipBallPositionAPI.apiInfo

	local loGetEngineInfoAPI = LoGetEngineInfoAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetEngineInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetEngineInfoAPI.apiInfo

	local loGetMechInfoAPI = LoGetMechInfoAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetMechInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetMechInfoAPI.apiInfo

	local loGetControlPanel_HSI_API = LoGetControlPanel_HSI_API:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetControlPanel_HSI_API
	self.apiTable[#self.apiTable + 1] = loGetControlPanel_HSI_API.apiInfo

	local loGetPayloadInfoAPI = LoGetPayloadInfoAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetPayloadInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetPayloadInfoAPI.apiInfo

	local loGetNavigationInfoAPI = LoGetNavigationInfoAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetNavigationInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetNavigationInfoAPI.apiInfo

	local loGetMagneticYawAPI = LoGetMagneticYawAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetMagneticYawAPI
	self.apiTable[#self.apiTable + 1] = loGetMagneticYawAPI.apiInfo

	local loGetBasicAtmospherePressureAPI = LoGetBasicAtmospherePressureAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetBasicAtmospherePressureAPI
	self.apiTable[#self.apiTable + 1] = loGetBasicAtmospherePressureAPI.apiInfo

	local loGetMCPStateAPI = LoGetMCPStateAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetMCPStateAPI
	self.apiTable[#self.apiTable + 1] = loGetMCPStateAPI.apiInfo

	local loGetTWSInfoAPI = LoGetTWSInfoAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetTWSInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetTWSInfoAPI.apiInfo

	local loGetAngleOfSideSlipAPI = LoGetAngleOfSideSlipAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetAngleOfSideSlipAPI
	self.apiTable[#self.apiTable + 1] = loGetAngleOfSideSlipAPI.apiInfo

	local loGetRadarAltimeterAPI = LoGetRadarAltimeterAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetRadarAltimeterAPI
	self.apiTable[#self.apiTable + 1] = loGetRadarAltimeterAPI.apiInfo

	local loGetRouteAPI = LoGetRouteAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetRouteAPI
	self.apiTable[#self.apiTable + 1] = loGetRouteAPI.apiInfo

	local loGetWingInfoAPI = LoGetWingInfoAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetWingInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetWingInfoAPI.apiInfo

	local loGetRadioBeaconsStatusAPI = LoGetRadioBeaconsStatusAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetRadioBeaconsStatusAPI
	self.apiTable[#self.apiTable + 1] = loGetRadioBeaconsStatusAPI.apiInfo

	local loGetVectorVelocityAPI = LoGetVectorVelocityAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetVectorVelocityAPI
	self.apiTable[#self.apiTable + 1] = loGetVectorVelocityAPI.apiInfo

	local loGetVectorWindVelocityAPI = LoGetVectorWindVelocityAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetVectorWindVelocityAPI
	self.apiTable[#self.apiTable + 1] = loGetVectorWindVelocityAPI.apiInfo

	local loGetAngularVelocityAPI = LoGetAngularVelocityAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetAngularVelocityAPI
	self.apiTable[#self.apiTable + 1] = loGetAngularVelocityAPI.apiInfo

	local loGetFMDataAPI = LoGetFMDataAPI:new(nil, counter())
	self.commandsTable[#self.commandsTable + 1] = loGetFMDataAPI
	self.apiTable[#self.apiTable + 1] = loGetFMDataAPI.apiInfo

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
