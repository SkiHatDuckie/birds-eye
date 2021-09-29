-- This script will be executed in BizHawk lua console
-- socket timeout values are in milliseconds

local status = "Stopped"

-- main loop
while true do
    -- display connection status
    gui.text(0, 20, "Hook Status: "..status)

    -- next frame!
    emu.frameadvance()
end
