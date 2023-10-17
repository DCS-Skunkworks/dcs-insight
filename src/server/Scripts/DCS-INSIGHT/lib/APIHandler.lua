module("APIHandler", package.seeall)

local LogInsight = require("Scripts.DCS-INSIGHT.lib.common.LogInsight")

local GetArgumentValue = require("Scripts.DCS-INSIGHT.lib.commands.GetArgumentValue")
local SetArgumentValue = require("Scripts.DCS-INSIGHT.lib.commands.SetArgumentValue")
local SetCommand = require("Scripts.DCS-INSIGHT.lib.commands.SetCommand")
local SetFrequency = require("Scripts.DCS-INSIGHT.lib.commands.SetFrequency")
local GetFrequency = require("Scripts.DCS-INSIGHT.lib.commands.GetFrequency")
local PerformClickableAction = require("Scripts.DCS-INSIGHT.lib.commands.PerformClickableAction")
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

--- @func Fills the commands and api table
function APIHandler:init()
	local getArgumentValue = GetArgumentValue:new(nil)
	self.commandsTable[#self.commandsTable + 1] = getArgumentValue
	self.apiTable[#self.apiTable + 1] = getArgumentValue.apiInfo

	local setArgumentValue = SetArgumentValue:new()
	self.commandsTable[#self.commandsTable + 1] = setArgumentValue
	self.apiTable[#self.apiTable + 1] = setArgumentValue.apiInfo

	local performClickableAction = PerformClickableAction:new()
	self.commandsTable[#self.commandsTable + 1] = performClickableAction
	self.apiTable[#self.apiTable + 1] = performClickableAction.apiInfo

	local setCommand = SetCommand:new()
	self.commandsTable[#self.commandsTable + 1] = setCommand
	self.apiTable[#self.apiTable + 1] = setCommand.apiInfo

	local getFrequency = GetFrequency:new()
	self.commandsTable[#self.commandsTable + 1] = getFrequency
	self.apiTable[#self.apiTable + 1] = getFrequency.apiInfo

	local setFrequency = SetFrequency:new()
	self.commandsTable[#self.commandsTable + 1] = setFrequency
	self.apiTable[#self.apiTable + 1] = setFrequency.apiInfo

	local loGetAircraftDrawArgumentValue = LoGetAircraftDrawArgumentValueAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetAircraftDrawArgumentValue
	self.apiTable[#self.apiTable + 1] = loGetAircraftDrawArgumentValue.apiInfo

	local listIndicationAPI = ListIndicationAPI:new()
	self.commandsTable[#self.commandsTable + 1] = listIndicationAPI
	self.apiTable[#self.apiTable + 1] = listIndicationAPI.apiInfo

	local listCockpitParamsAPI = ListCockpitParamsAPI:new(nil)
	self.commandsTable[#self.commandsTable + 1] = listCockpitParamsAPI
	self.apiTable[#self.apiTable + 1] = listCockpitParamsAPI.apiInfo

	local loGetSelfData = LoGetSelfDataAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetSelfData
	self.apiTable[#self.apiTable + 1] = loGetSelfData.apiInfo

	local loGetModelTimeAPI = LoGetModelTimeAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetModelTimeAPI
	self.apiTable[#self.apiTable + 1] = loGetModelTimeAPI.apiInfo

	local loGetMissionStartTimeAPI = LoGetMissionStartTimeAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetMissionStartTimeAPI
	self.apiTable[#self.apiTable + 1] = loGetMissionStartTimeAPI.apiInfo

	local loIsOwnshipExportAllowedAPI = LoIsOwnshipExportAllowedAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loIsOwnshipExportAllowedAPI
	self.apiTable[#self.apiTable + 1] = loIsOwnshipExportAllowedAPI.apiInfo

	local loGetPilotName = LoGetPilotNameAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetPilotName
	self.apiTable[#self.apiTable + 1] = loGetPilotName.apiInfo

	local loGetIndicatedAirSpeedAPI = LoGetIndicatedAirSpeedAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetIndicatedAirSpeedAPI
	self.apiTable[#self.apiTable + 1] = loGetIndicatedAirSpeedAPI.apiInfo

	local loGetAccelerationUnitsAPI = LoGetAccelerationUnitsAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetAccelerationUnitsAPI
	self.apiTable[#self.apiTable + 1] = loGetAccelerationUnitsAPI.apiInfo

	local loGetADIPitchBankYawAPI = LoGetADIPitchBankYawAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetADIPitchBankYawAPI
	self.apiTable[#self.apiTable + 1] = loGetADIPitchBankYawAPI.apiInfo

	local loGetSnaresAPI = LoGetSnaresAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetSnaresAPI
	self.apiTable[#self.apiTable + 1] = loGetSnaresAPI.apiInfo

	local loGetAltitudeAboveSeaLevelAPI = LoGetAltitudeAboveSeaLevelAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetAltitudeAboveSeaLevelAPI
	self.apiTable[#self.apiTable + 1] = loGetAltitudeAboveSeaLevelAPI.apiInfo

	local loGetAltitudeAboveGroundLevelAPI = LoGetAltitudeAboveGroundLevelAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetAltitudeAboveGroundLevelAPI
	self.apiTable[#self.apiTable + 1] = loGetAltitudeAboveGroundLevelAPI.apiInfo

	local loGetVerticalVelocityAPI = LoGetVerticalVelocityAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetVerticalVelocityAPI
	self.apiTable[#self.apiTable + 1] = loGetVerticalVelocityAPI.apiInfo

	local loGetTrueAirSpeedAPI = LoGetTrueAirSpeedAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetTrueAirSpeedAPI
	self.apiTable[#self.apiTable + 1] = loGetTrueAirSpeedAPI.apiInfo

	local loGetMachNumberAPI = LoGetMachNumberAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetMachNumberAPI
	self.apiTable[#self.apiTable + 1] = loGetMachNumberAPI.apiInfo

	local loGetAngleOfAttackAPI = LoGetAngleOfAttackAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetAngleOfAttackAPI
	self.apiTable[#self.apiTable + 1] = loGetAngleOfAttackAPI.apiInfo

	local loGetGlideDeviationAPI = LoGetGlideDeviationAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetGlideDeviationAPI
	self.apiTable[#self.apiTable + 1] = loGetGlideDeviationAPI.apiInfo

	local loGetSideDeviationAPI = LoGetSideDeviationAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetSideDeviationAPI
	self.apiTable[#self.apiTable + 1] = loGetSideDeviationAPI.apiInfo

	local loGetSlipBallPositionAPI = LoGetSlipBallPositionAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetSlipBallPositionAPI
	self.apiTable[#self.apiTable + 1] = loGetSlipBallPositionAPI.apiInfo

	local loGetEngineInfoAPI = LoGetEngineInfoAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetEngineInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetEngineInfoAPI.apiInfo

	local loGetMechInfoAPI = LoGetMechInfoAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetMechInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetMechInfoAPI.apiInfo

	local loGetControlPanel_HSI_API = LoGetControlPanel_HSI_API:new()
	self.commandsTable[#self.commandsTable + 1] = loGetControlPanel_HSI_API
	self.apiTable[#self.apiTable + 1] = loGetControlPanel_HSI_API.apiInfo

	local loGetPayloadInfoAPI = LoGetPayloadInfoAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetPayloadInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetPayloadInfoAPI.apiInfo

	local loGetNavigationInfoAPI = LoGetNavigationInfoAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetNavigationInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetNavigationInfoAPI.apiInfo

	local loGetMagneticYawAPI = LoGetMagneticYawAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetMagneticYawAPI
	self.apiTable[#self.apiTable + 1] = loGetMagneticYawAPI.apiInfo

	local loGetBasicAtmospherePressureAPI = LoGetBasicAtmospherePressureAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetBasicAtmospherePressureAPI
	self.apiTable[#self.apiTable + 1] = loGetBasicAtmospherePressureAPI.apiInfo

	local loGetMCPStateAPI = LoGetMCPStateAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetMCPStateAPI
	self.apiTable[#self.apiTable + 1] = loGetMCPStateAPI.apiInfo

	local loGetTWSInfoAPI = LoGetTWSInfoAPI:new()
	self.commandsTable[#self.commandsTable + 1] = loGetTWSInfoAPI
	self.apiTable[#self.apiTable + 1] = loGetTWSInfoAPI.apiInfo

	self:verify_entries()
end

--- @func Executes the command and returns a command containing the result
--- @param api APIInfo
function APIHandler:execute(api)
	for k, v in pairs(self.commandsTable) do
		if api.id == v.id then
			local result_code, result = pcall(v.execute, v, api) -- = v:execute(api)
			if result_code == true then
				return result
			else
				if result == nil then
					api.result = "Error but no error message"
					return api
				else
					local path = v:script_path():gsub("%-", "%%-") -- escape any hyphen otherwise next gsub won't work
					LogInsight:log(result:gsub(path, ""))
					api.result = result:gsub(path, "")
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
	LogInsight:log(message)

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
			LogInsight:log("Parameter count mismatch in " .. command.apiInfo.api_syntax)
		end
	end

	local message = "Following api have duplicate id. This must be corrected :\n"
	local found = false
	for i = 1, #duplicated do
		message = message .. duplicated[i].id .. "  " .. duplicated[i].api_syntax .. "\n"
		found = true
	end

	if found then
		LogInsight:log(message)
		error("dcs-insight API Id conflicts found")
	end

	return true
end

return APIHandler
