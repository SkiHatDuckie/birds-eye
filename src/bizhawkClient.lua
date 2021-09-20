-- This script will be executed in BizHawk lua console

while true do
    -- send message to server
    comm.socketServerSend("Client-Server Status: Excelent :]\r\n")

    -- check for messages from server
    local msg = comm.socketServerResponse()
    if msg ~= nil then
        print(msg)
        break
    end
end
