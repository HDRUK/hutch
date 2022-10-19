import os
import sys
import logging
import dotenv
import hutchagent.config as config
from hutchagent.db_manager import SyncDBManager
from hutchagent.db_logging import SyncLogDBHandler
from hutchagent.checkin import CheckIn
from azure.storage.queue import QueueClient

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
    check_in_thread.start()

    # TODO: queue stuff here

    if check_in_thread.is_alive():
        # stop check-in thred
        check_in_thread.join()

    logger.info("Successfully shut down :)")


if __name__ == "__main__":
    main()
