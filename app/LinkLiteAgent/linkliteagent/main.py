import sys
import os
import logging
import time
import asyncio


async def loop():
    """The main method"""
    running = True
    # send the logging to stdout
    handler = logging.StreamHandler(sys.stdout)
    # set up the logger
    logger = logging.getLogger("linkliteagent")
    logger.setLevel(logging.INFO)
    logger.addHandler(handler)

    while running:
        try:
            # hello world
            logger.info(f"Hello World @ {time.ctime()}")
            time.sleep(5)
        except KeyboardInterrupt:
            # shut down on Ctrl+C
            logger.warning(f"{os.linesep}Shut down @ {time.ctime()}")
            running = False


def main():
    asyncio.run(loop())


if __name__ == "__main__":
    main()
