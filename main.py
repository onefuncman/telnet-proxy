import signal
import asyncio
from typing import List
from enum import Enum
import telnetlib

from telnet_proxy import TelnetProxy


class Direction(Enum):
    ToMud = 1
    FromMud = 2


async def begin(handler, proxy):
    local_end_point = handler.getpeername()
    try:
        proxy.start(handler, local_end_point)
    except Exception as e:
        print(f"error handling connection from {local_end_point}: {e}")
    finally:
        handler.close()
        print(f"closed connection from: {local_end_point}")


async def start(proxy):
    server = await asyncio.start_server(
        lambda r, w: asyncio.ensure_future(begin(r, proxy)),
        '0.0.0.0', 1080)
    async with server:
        print('Proxy started.')
        await server.serve_forever()


async def main():
    tn = telnetlib.Telnet()
    tn.open('localhost', 23)
    proxy = TelnetProxy(tn)
    loop = asyncio.get_event_loop()
    try:
        await asyncio.gather(start(proxy), loop=loop)
    except asyncio.CancelledError:
        pass
    finally:
        tn.close()
        loop.stop()


if __name__ == "__main__":
    loop = asyncio.get_event_loop()
    signal.signal(signal.SIGINT, lambda s, f: loop.create_task(main().cancel()))
    try:
        loop.run_until_complete(main())
    finally:
        loop.close()

