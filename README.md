# dcs-insight* 

[![license](https://img.shields.io/github/license/DCS-Skunkworks/dcs-insight.svg)](https://github.com/DCS-Skunkworks/dcs-insight/blob/main/LICENSE)
[![GitHub release](https://img.shields.io/github/release/DCS-Skunkworks/dcs-insight.svg)](https://github.com/DCS-Skunkworks/dcs-insight/releases)
[![Discord](https://img.shields.io/discord/533342958712258572)](https://discord.gg/5svGwKX)

Application for executing DCS API.

You can test ```manually``` or do ```range testing``` to figure out how devices / arguments etc behave.

### Server
Install server in ```Scripts``` folder and add entry to Export.lua.

```dofile(lfs.writedir()..[[Scripts\DCS-INSIGHT\lib\DCSInsight.lua]])```

![scripts_folders](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/e4ee8c96-4c57-42f5-b9ef-a2edc6c0484f)

* Start the mission
* Connect client, once connected dcs-insight server sends all APIs it has
* You can poll an API for changes
* You can search for API

* 1 : GetDevice(device_id):get_argument_value(argument_id)
* 2 : GetDevice(device_id):set_argument_value(argument_id, new_value)
* 3 : GetDevice(device_id):performClickableAction(command_id, argument_id)
* 4 : GetDevice(device_id):SetCommandAPI(command_id, new_value)
* 5 : GetDevice(device_id):get_frequency()
* 6 : GetDevice(device_id):set_frequency(new_value)
* 7 : GetDevice(device_id):update_arguments()
* 8 : LoGetAircraftDrawArgumentValue(draw_argument_id)
* 9 : LoGetObjectById(object_id)
* 10 : list_indication(indicator_id)
* 11 : LoSetCommand(iCommand)
* 12 : LoSetCommand(iCommand, new_value)
* 13 : LoGeoCoordinatesToLoCoordinates(longitude_degrees, latitude_degrees)
* 14 : LoCoordinatesToGeoCoordinates(x, z)
* 15 : list_cockpit_params()
* 16 : LoGetSelfData()
* 17 : LoGetModelTime()
* 18 : LoGetMissionStartTime()
* 19 : LoGetPilotName()
* 20 : LoGetIndicatedAirSpeed()
* 21 : LoGetAccelerationUnits()
* 22 : LoGetADIPitchBankYaw()
* 23 : LoGetSnares()
* 24 : LoGetAltitudeAboveSeaLevel()
* 25 : LoGetAltitudeAboveGroundLevel()
* 26 : LoGetVerticalVelocity()
* 27 : LoGetTrueAirSpeed()
* 28 : LoGetMachNumber()
* 29 : LoGetAngleOfAttack()
* 30 : LoGetGlideDeviation()
* 31 : LoGetSideDeviation()
* 32 : LoGetSlipBallPosition()
* 33 : LoGetEngineInfo()
* 34 : LoGetMechInfo()
* 35 : LoGetControlPanel_HSI()
* 36 : LoGetPayloadInfo()
* 37 : LoGetNavigationInfo()
* 38 : LoGetMagneticYaw()
* 39 : LoGetBasicAtmospherePressure()
* 40 : LoGetMCPState()
* 41 : LoGetTWSInfo()
* 42 : LoGetAngleOfSideSlip()
* 43 : LoGetRadarAltimeter()
* 44 : LoGetRoute()
* 45 : LoGetWingInfo()
* 46 : LoGetRadioBeaconsStatus()
* 47 : LoGetVectorVelocity()
* 48 : LoGetVectorWindVelocity()
* 49 : LoGetAngularVelocity()
* 50 : LoGetFMData()
* 51 : LoGetWorldObjects()
* 52 : LoGetTargetInformation()
* 53 : LoGetLockedTargetInformation()
* 54 : LoGetF15_TWS_Contacts()
* 55 : LoGetSightingSystemInfo()
* 56 : LoGetWingTargets()
* 57 : LoGetAltitude()
* 58 : LoIsOwnshipExportAllowed()
* 59 : LoIsObjectExportAllowed()
* 60 : LoIsSensorExportAllowed()

![insight1](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/a99b72b9-b13b-4b3e-b35b-35188b907bd5)

![insight2](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/3cf176f8-011b-43b6-bc44-4b84feefa9a0)

![insight3](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/033d63a1-0757-4323-b6e2-63dda6f8b5c9)

![range_test](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/67f5a7cc-1cc9-4f71-b92e-6dd1560eb100)




