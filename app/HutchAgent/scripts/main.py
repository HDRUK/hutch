import os
import sys
import logging
import hutchagent.config as config
import pika
import dotenv
import hutchagent.message_queues.rmq_queue as rmq
from hutchagent.checkin import CheckIn


def main():
    """The main method"""
    dotenv.load_dotenv()  # load .env values into environment

    LOG_FORMAT = logging.Formatter(
        config.MSG_FORMAT,
        datefmt=config.DATE_FORMAT,
    )

    # set up the logger
    handler = logging.StreamHandler(sys.stdout)
    handler.setFormatter(LOG_FORMAT)
    logger = logging.getLogger(config.LOGGER_NAME)
    logger.setLevel(logging.INFO)
    logger.addHandler(handler)

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
        channel = rmq.connect(
            queue=os.getenv("DATASOURCE_NAME"),
            host=os.getenv("MSG_QUEUE_HOST", "localhost"),
            heartbeat=300,
        )
        channel.basic_consume(
            os.getenv("DATASOURCE_NAME"),
            on_message_callback=rmq.ro_crates_callback,
            auto_ack=True,
        )
        logger.info("Successfully connected to queue. Press Ctrl+C to exit.")
        channel.start_consuming()  # starts a `while True` loop.
    except pika.exceptions.AMQPConnectionError:
        logger.critical("Unable to connect to queue. Exiting...")
    except KeyboardInterrupt:
        # shut down on Ctrl+C
        logger.info("Disconnecting from queue...")
        if channel.connection.is_open:
            rmq.disconnect(channel)

    if check_in_thread.is_alive():
        # stop check-in thred
        check_in_thread.join()

    logger.info("Successfully shut down :)")


if __name__ == "__main__":
    main()
