module("LogInsight", package.seeall)

Logger = require("Logger")
--- @type Logger
local Log = Logger:new(lfs.writedir()..[[Logs\dcs-insight-server.log]])

return Log
