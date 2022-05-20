import sys
import os
import logging
import time
import pika
import linkliteagent.message_queue as mq
from linkliteagent.db_manager import SyncDBManager
from linkliteagent.db_logging import SyncLogDBHandler


async def async_main():
    """An asynchronous version of the main function"""
    pass


def main():
    """The main method"""
    format = logging.Formatter(
        "%(levelname)s - %(asctime)s - %(message)s",
        datefmt="%d-%b-%y %H:%M:%S",
    )

    # set up the backup logger
    backup_handler = logging.StreamHandler(sys.stdout)
    backup_handler.setFormatter(format)
    backup_logger = logging.getLogger("backup_logger")
    backup_logger.setLevel(logging.INFO)
    backup_logger.addHandler(backup_handler)

    # set up the db logger
    db_manager = SyncDBManager(
        drivername="postgresql+psycopg2",
        username="postgres",
        password="example",
        host="localhost",
        port=5432,
        database="postgres",
    )
    db_handler = SyncLogDBHandler(db_manager, "backup_logger")
    db_handler.setFormatter(format)
    db_logger = logging.getLogger("db_logger")
    db_logger.setLevel(logging.INFO)
    db_logger.addHandler(db_handler)
    db_logger.addHandler(backup_handler)

    # Connect to RabbitMQ
    try:
        db_logger.info("Connecting to queue.")
        channel = mq.connect("hello")
        db_logger.info("Successfully connected to queue.")
    except pika.exceptions.AMQPConnectionError:
        db_logger.critical("Unable to connect to queue. Exiting...", exc_info=True)
        exit(-1)

    running = True
    while running:
        try:
            # hello world
            db_logger.info(f"Hello World @ {time.ctime()}")
            time.sleep(2)
        except KeyboardInterrupt:
            # shut down on Ctrl+C
            db_logger.warning(f"Shut down @ {time.ctime()}")
            running = False


if __name__ == "__main__":
    main()
