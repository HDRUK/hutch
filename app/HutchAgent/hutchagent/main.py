import os
import sys
import logging
import hutchagent.config as config
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
        config.MSG_FORMAT,
        datefmt=config.DATE_FORMAT,
    )

    # set up the backup logger
    console_handler = logging.StreamHandler(sys.stdout)
    console_handler.setFormatter(LOG_FORMAT)
    backup_logger = logging.getLogger(config.BACKUP_LOGGER_NAME)
    backup_logger.setLevel(logging.INFO)
    backup_logger.addHandler(console_handler)

    # set up the db logger
    log_db_host = os.getenv("LOG_DB_HOST")
    log_db_port = os.getenv("LOG_DB_PORT")
    
    logger = logging.getLogger(config.LOGGER_NAME)
    logger.setLevel(logging.INFO)
    if log_db_host is not None:
        db_manager = SyncDBManager(
            username=os.getenv("LOG_DB_USERNAME"),
            password=os.getenv("LOG_DB_PASSWORD"),
            host=log_db_host,
            port=int(log_db_port) if log_db_port is not None else None,
            database=os.getenv("LOG_DB_DATABASE"),
            drivername=os.getenv("LOG_DB_DRIVERNAME", config.DEFAULT_DB_DRIVER),
        )
        db_handler = SyncLogDBHandler(db_manager, config.BACKUP_LOGGER_NAME)
        db_handler.setFormatter(LOG_FORMAT)
    
        logger.addHandler(db_handler)
        
    logger.addHandler(console_handler)

    # set up check-in thread
    check_in_thread = CheckIn(
        cron=os.getenv("CHECKIN_CRON", "0 */1 * * *"),
        url=f"{os.getenv('MANAGER_URL')}/api/agents/checkin",
        data_source_id=os.getenv("DATASOURCE_NAME"),
    )

    # Connect to RabbitMQ
    try:
        check_in_thread.start()
        logger.info("Connecting to queue.")
        channel = mq.connect(os.getenv("DATASOURCE_NAME"))
        channel.basic_consume(
            os.getenv("DATASOURCE_NAME"),
            on_message_callback=(
                mq.ro_crates_callback
                if int(os.getenv("USE_RO_CRATES", 0))
                else mq.rquest_callback
            ),
            auto_ack=True,
        )
        logger.info("Successfully connected to queue. Press Ctrl+C to exit.")
        channel.start_consuming()  # starts a `while True` loop.
    except pika.exceptions.AMQPConnectionError:
        logger.critical("Unable to connect to queue. Exiting...", exc_info=True)
    except KeyboardInterrupt:
        # shut down on Ctrl+C
        logger.info("Disconnecting from queue...")
        if channel.connection.is_open:
            mq.disconnect(channel)

    if check_in_thread.is_alive():
        # stop check-in thred
        check_in_thread.join()

    logger.info("Successfully shut down :)")


if __name__ == "__main__":
    main()
