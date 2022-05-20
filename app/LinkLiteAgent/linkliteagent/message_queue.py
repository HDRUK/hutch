import pika
from pika.adapters.blocking_connection import BlockingChannel


def connect(queue: str, host="localhost", **kwargs) -> BlockingChannel:
    """Connect to a RabbitMQ instance.

    Args:
        queue (str): The name of the queue to consume.
        host (str, optional): The host's address. Defaults to "localhost".
        **kwargs: Any other parameters to be passed to `pika.ConnectionParameters`.
        [More info](https://pika.readthedocs.io/en/stable/modules/parameters.html)

    Returns:
        BlockingChannel: A channel connected to the queue.
    """
    connection = pika.BlockingConnection(
        pika.ConnectionParameters(host, **kwargs)
    )
    channel = connection.channel()
    channel.queue_declare(queue=queue)
    return channel
