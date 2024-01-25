module("CommonInsight", package.seeall)

local CommonInsight = {}

--- Checks is module is loaded
--- @param module_name string
--- @return boolean
function CommonInsight:isModuleAvailable(module_name)
	if package.loaded[module_name] then
		return true
	else
		for _, searcher in ipairs(package.searchers or package.loaders) do
			local loader = searcher(module_name)
			if type(loader) == "function" then
				package.preload[module_name] = loader
				return true
			end
		end
		return false
	end
end

--- Tries to require the module
--- @param module_name string
--- @return boolean
function CommonInsight:tryRequire(module_name)
	local return_code, module = pcall(require, module_name)
	if return_code then
		return true
	end

	return false
end

return CommonInsight
