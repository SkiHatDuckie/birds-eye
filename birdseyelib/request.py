class Request:
    def __init__(self, tag, client) -> None:
        self.tag = tag
        self.client = client

    def queue(self):
        self.client._queue_request(self.tag + ";\n")


class RequestBatch:
    def __init__(self, request_objects) -> None:
        self.request_objects = request_objects

    def queue_all(self):
        for req in self.request_objects:
            req.queue()
