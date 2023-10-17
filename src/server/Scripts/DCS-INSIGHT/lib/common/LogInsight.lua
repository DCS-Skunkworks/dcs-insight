module("LogInsight", package.seeall)

LoggerIns = require("Scripts.DCS-INSIGHT.lib.common.LoggerIns")
--- @type LoggerIns
local Log = LoggerIns:new(lfs.writedir() .. [[Logs\dcs-insight-server.log]])

return Log
