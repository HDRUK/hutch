import sys
import logging
import pika
import linkliteagent.message_queue as mq
from linkliteagent.db_manager import SyncDBManager
from linkliteagent.db_logging import SyncLogDBHandler
from linkliteagent.query import query_callback


async def async_main():
    """An asynchronous version of the main function"""
    pass


def main():
    QUEUE_NAME = "link-like-queue"

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
        channel = mq.connect(QUEUE_NAME)
        channel.basic_consume(QUEUE_NAME, on_message_callback=query_callback)
        db_logger.info("Successfully connected to queue. Press Ctrl+C to exit.")
        channel.start_consuming()  # starts a `while True` loop.
    except pika.exceptions.AMQPConnectionError:
        db_logger.critical("Unable to connect to queue. Exiting...", exc_info=True)
        exit(-1)
    except KeyboardInterrupt:
        # shut down on Ctrl+C
        db_logger.info("Disconnecting from queue...")
        if channel.connection.is_open:
            mq.disconnect(channel)

    db_logger.info("Successfully shut down :)")


if __name__ == "__main__":
    main()
