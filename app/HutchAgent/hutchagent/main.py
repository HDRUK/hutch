import os
import sys
import logging
import pika
import dotenv
import hutchagent.message_queue as mq
from hutchagent.db_manager import SyncDBManager
from hutchagent.db_logging import SyncLogDBHandler
from hutchagent.checkin import CheckIn


async def async_main():
    """An asynchronous version of the main function"""
    pass


def main():
    """The main method"""
    dotenv.load_dotenv()  # load .env values into environment

    LOG_FORMAT = logging.Formatter(
        os.getenv("MSG_FORMAT"),
        datefmt=os.getenv("DATE_FORMAT"),
    )

    # set up the backup logger
    backup_handler = logging.StreamHandler(sys.stdout)
    backup_handler.setFormatter(LOG_FORMAT)
    backup_logger = logging.getLogger(os.getenv("BACKUP_LOGGER_NAME"))
    backup_logger.setLevel(logging.INFO)
    backup_logger.addHandler(backup_handler)

    # set up the db logger
    db_manager = SyncDBManager(
        username=os.getenv("LOG_DB_USERNAME"),
        password=os.getenv("LOG_DB_PASSWORD"),
        host=os.getenv("LOG_DB_HOST"),
        port=int(os.getenv("LOG_DB_PORT")),
        database=os.getenv("LOG_DB_DATABASE"),
        drivername=os.getenv("LOG_DB_DRIVERNAME"),
    )
    db_handler = SyncLogDBHandler(db_manager, os.getenv("BACKUP_LOGGER_NAME"))
    db_handler.setFormatter(LOG_FORMAT)
    db_logger = logging.getLogger(os.getenv("DB_LOGGER_NAME"))
    db_logger.setLevel(logging.INFO)
    db_logger.addHandler(db_handler)
    db_logger.addHandler(backup_handler)

    # set up check-in thread
    check_in_thread = CheckIn(
        cron=os.getenv("CRON_STRING"),
        url=f"{os.getenv('MANAGER_URL')}/api/agents/checkin",
        data_source_id=os.getenv("DATASOURCE_ID"),
    )

    # Connect to RabbitMQ
    try:
        check_in_thread.start()
        db_logger.info("Connecting to queue.")
        channel = mq.connect(os.getenv("DATASOURCE_QUEUE_NAME"))
        channel.basic_consume(
            os.getenv("DATASOURCE_QUEUE_NAME"),
            on_message_callback=(
                mq.ro_crates_callback
                if int(os.getenv("USE_RO_CRATES", 0))
                else mq.rquest_callback
            ),
            auto_ack=True,
        )
        db_logger.info("Successfully connected to queue. Press Ctrl+C to exit.")
        channel.start_consuming()  # starts a `while True` loop.
    except pika.exceptions.AMQPConnectionError:
        db_logger.critical("Unable to connect to queue. Exiting...", exc_info=True)
    except KeyboardInterrupt:
        # shut down on Ctrl+C
        db_logger.info("Disconnecting from queue...")
        if channel.connection.is_open:
            mq.disconnect(channel)

    if check_in_thread.is_alive():
        # stop check-in thred
        check_in_thread.join()

    db_logger.info("Successfully shut down :)")


if __name__ == "__main__":
    main()
