import socket
import threading
import asyncio
from typing import List
from enum import Enum

class Direction(Enum):
    ToMud = 1
    FromMud = 2

def begin_receive(buffer, direction, origin, destination, waiter):
    def on_receive(result):
        read = origin.recv(len(buffer))
        if not read or not destination:
            origin.close()
            waiter.set()
            return

        destination.sendall(read)
        origin.settimeout(0)
        origin.recv_into(buffer)
        origin.settimeout(None)

    origin.settimeout(None)
    origin.setblocking(True)
    threading.Thread(target=origin.recv_into, args=(buffer, len(buffer))).start()

async def begin(handler):
    local_end_point = handler.getpeername()
    proxy_address = "217.180.196.241" #"greatermud.com"
    proxy_ip = socket.gethostbyname(proxy_address)
    mud_end_point = (proxy_ip, 2427)

    outbound = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    outbound.settimeout(5)
    outbound.connect(mud_end_point)
    outbound.setblocking(True)

    waiter = threading.Event()
    buffer = bytearray(1024)
    begin_receive(buffer, Direction.ToMud, handler, outbound, waiter)
    begin_receive(buffer, Direction.FromMud, outbound, handler, waiter)

    await waiter.wait()
    outbound.close()

    print(f"closed connection from: {local_end_point}")

async def start():
    inbound = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    inbound.bind(('0.0.0.0', 1080))
    inbound.listen(10)

    sessions = []
    while True:
        handler, _ = await inbound.accept()
        print(f"received connection from: {handler.getpeername()}")
        sessions.append(asyncio.create_task(begin(handler)))

if __name__ == "__main__":
    asyncio.run(start())

