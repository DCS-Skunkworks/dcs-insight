module("Log", package.seeall)

Logger = require("Scripts.DCS-INSIGHT.lib.common.Logger")
--- @type Logger
local Log = Logger:new(lfs.writedir() .. [[Logs\dcs-insight-server.log]])

return Log
