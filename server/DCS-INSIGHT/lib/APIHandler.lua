module("APIHandler", package.seeall)

local GetArgumentValue = require("GetArgumentValue")
local SetArgumentValue = require("SetArgumentValue")
local SetCommand = require("SetCommand")
local SetFrequency = require("SetFrequency")
local GetFrequency = require("GetFrequency")
local PerformClickableAction = require("PerformClickableAction")
local LoGetAircraftDrawArgumentValueAPI = require("LoGetAircraftDrawArgumentValueAPI")
local LoGetSelfDataAPI = require("LoGetSelfDataAPI")
local LoGetModelTimeAPI = require("LoGetModelTimeAPI")
local LoGetMissionStartTimeAPI = require("LoGetMissionStartTimeAPI")
local LoIsOwnshipExportAllowedAPI = require("LoIsOwnshipExportAllowedAPI")
local LoGetPilotNameAPI = require("LoGetPilotNameAPI")
local LoGetIndicatedAirSpeedAPI = require("LoGetIndicatedAirSpeedAPI")
local LoGetAccelerationUnitsAPI = require("LoGetAccelerationUnitsAPI")
local LoGetADIPitchBankYawAPI = require("LoGetADIPitchBankYawAPI")
local LoGetSnaresAPI = require("LoGetSnaresAPI")
local ListIndicationAPI = require("ListIndicationAPI")
local ListCockpitParamsAPI = require("ListCockpitParamsAPI")
local LoGetAltitudeAboveSeaLevelAPI = require("LoGetAltitudeAboveSeaLevelAPI")
local LoGetAltitudeAboveGroundLevelAPI = require("LoGetAltitudeAboveGroundLevelAPI")
local LoGetVerticalVelocityAPI = require("LoGetVerticalVelocityAPI")
local LoGetTrueAirSpeedAPI = require("LoGetTrueAirSpeedAPI")
local LoGetMachNumberAPI = require("LoGetMachNumberAPI")
local LoGetAngleOfAttackAPI = require("LoGetAngleOfAttackAPI")
local LoGetGlideDeviationAPI = require("LoGetGlideDeviationAPI")
local LoGetSideDeviationAPI = require("LoGetSideDeviationAPI")
local LoGetSlipBallPositionAPI = require("LoGetSlipBallPositionAPI")
local LoGetEngineInfoAPI = require("LoGetEngineInfoAPI")
local LoGetMechInfoAPI = require("LoGetMechInfoAPI")






--- @class APIHandler 
--- @field public commandsTable table<APIBase>
--- @field public apiTable table<APIInfo>
local APIHandler = {}

--- @func Returns new APIHandler
function APIHandler:new()
    --- @type APIHandler
    local o = {
        commandsTable = {},
        apiTable = {}

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
    
    local setCommand = SetCommand:new()
    self.commandsTable[#self.commandsTable + 1] = setCommand
    self.apiTable[#self.apiTable + 1] = setCommand.apiInfo

    local getFrequency = GetFrequency:new()
    self.commandsTable[#self.commandsTable + 1] = getFrequency
    self.apiTable[#self.apiTable + 1] = getFrequency.apiInfo

    local setFrequency = SetFrequency:new()
    self.commandsTable[#self.commandsTable + 1] = setFrequency
    self.apiTable[#self.apiTable + 1] = setFrequency.apiInfo
    
    local performClickableAction = PerformClickableAction:new()
    self.commandsTable[#self.commandsTable + 1] = performClickableAction
    self.apiTable[#self.apiTable + 1] = performClickableAction.apiInfo

    local loGetAircraftDrawArgumentValue = LoGetAircraftDrawArgumentValueAPI:new()
    self.commandsTable[#self.commandsTable + 1] = loGetAircraftDrawArgumentValue
    self.apiTable[#self.apiTable + 1] = loGetAircraftDrawArgumentValue.apiInfo
    
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
    
    local listIndicationAPI = ListIndicationAPI:new()
    self.commandsTable[#self.commandsTable + 1] = listIndicationAPI
    self.apiTable[#self.apiTable + 1] = listIndicationAPI.apiInfo
    
    local listCockpitParamsAPI = ListCockpitParamsAPI:new(nil)
    self.commandsTable[#self.commandsTable + 1] = listCockpitParamsAPI
    self.apiTable[#self.apiTable + 1] = listCockpitParamsAPI.apiInfo
    
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
    
    self:verify_entries()
end

--- @func Executes the command and returns a command containing the result
--- @param api APIInfo
function APIHandler:execute(api)

    for k, v in pairs(self.commandsTable) do
        if (api.id == v.id) then

            local result_code, result = pcall(v.execute, v, api)
            if(result_code == true)then
                return result
            else
                api.result = result
                return api
            end
        end
    end
end


--- @func Performs checks on registered api
--- @return boolean
function APIHandler:verify_entries()
    
    local message = "Following api have been loaded :\n"
    for i = 1, #self.commandsTable do
        message = message..self.commandsTable[i].id.." : "..self.commandsTable[i].apiInfo.api_syntax.."\n"
    end
    Logg:log(message)

    local seen = {}
    local duplicated = {}
    for i = 1, #self.commandsTable do
        local command = self.commandsTable[i]
        if seen[command.id] then  --check if we've seen the element before
            duplicated[#duplicated+1] = {id = command.id, api_syntax = command.apiInfo.api_syntax} --if we have then it must be a duplicate! add to a table to keep track of this
        else
            seen[command.id] = true -- set the element to seen
        end

        if(command.apiInfo.parameter_count ~= #command.apiInfo.parameter_defs)then
            Logg:log("Parameter count mismatch in "..command.apiInfo.api_syntax)
        end
    end

    local message = "Following api have duplicate id. This must be corrected :\n"
    local found = false
    for i = 1, #duplicated do
        message = message..duplicated[i].id.."  "..duplicated[i].api_syntax.."\n"
        found = true
    end

    if(found)then
        Logg:log(message)
        error("dcs-insight API Id conflicts found")
    end

    return true
end


return APIHandler
