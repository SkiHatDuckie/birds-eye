-- main loop
while true do
    -- say hello to server
    comm.socketServerSend(emu.framecount().."\n")

    -- next frame!
    emu.frameadvance()
end
