# dcs-insight

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

![insight1](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/a99b72b9-b13b-4b3e-b35b-35188b907bd5)

![insight2](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/3cf176f8-011b-43b6-bc44-4b84feefa9a0)

![insight3](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/033d63a1-0757-4323-b6e2-63dda6f8b5c9)

![range_test](https://github.com/DCS-Skunkworks/dcs-insight/assets/10453261/67f5a7cc-1cc9-4f71-b92e-6dd1560eb100)




