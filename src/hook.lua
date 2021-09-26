-- This script will be executed in BizHawk lua console
-- socket timeout values are in milliseconds

local status = "Disconnected"

-- main loop
while true do
    -- display connection status
    gui.text(0, 20, "Hook Status: "..status)

    -- check if hook is connected to server
    if comm.socketServerIsConnected() then
        status = "Connected"
    else
        status = "Disconnect"
    end

    -- next frame!
    emu.frameadvance()
end
