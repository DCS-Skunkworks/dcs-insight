module("LogInsight", package.seeall)

LoggerIns = require("LoggerIns")
--- @type LoggerIns
local Log = LoggerIns:new(lfs.writedir() .. [[Logs\dcs-insight-server.log]])

return Log
