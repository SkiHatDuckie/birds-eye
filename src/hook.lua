-- This script will be executed in BizHawk lua console

-- send message to server
comm.socketServerSend("SOCKET WORKS! FUCK YEAH!\n")

while true do
    -- check for messages from server
    local msg = comm.socketServerResponse()
    if msg ~= nil then
        print(msg)
        break
    end
end
