require('o_mis')
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"


function onNotifyObject(self,msg)

	if msg.name == "reset" then
	
		self:RebuildReset()
	end


end



function onRebuildComplete(self, msg)

  
        	--------------------------------------
			--   Store Activity Object if nil   --
			--------------------------------------
			if not self:GetVar("ActivityObj") then
			
			 	local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
			 	storeObjectByName(self, "ActivityObj", ActivityObj[1])
			 	
			end
			
			local ActivityObj =  getObjectByName(self, "ActivityObj")
			

			ActivityObj:NotifyObject{ name = "EndRebuildReady" }
			
       
 
end 