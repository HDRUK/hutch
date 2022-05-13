import sys
import os
import logging
import time
import asyncio


async def main():
    """The main method. The program is run from here."""
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


if __name__ == "__main__":
    asyncio.run(main())
