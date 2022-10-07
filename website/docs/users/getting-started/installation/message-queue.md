---
sidebar_position: 4
---

# Message Queue

Hutch uses [RabbitMQ] for its Message Queue.

You can install RabbitMQ [however you want][install-rabbitmq], reusing an existing server if appropriate.

An easy way can be to use [Docker][rabbitmq-docker], configured via environment variables:

- `RABBITMQ_DEFAULT_USER=<username>`
- `RABBITMQ_DEFAULT_PASS=<password>`
  - **Generate a good random one!**

[RabbitMQ]: https://www.rabbitmq.com/
[install-rabbitmq]: https://www.rabbitmq.com/download.html
[rabbitmq-docker]: https://hub.docker.com/_/rabbitmq
