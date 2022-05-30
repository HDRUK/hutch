# Logging configuration
DB_LOGGER_NAME = "db"
BACKUP_LOGGER_NAME = "backup"
LOGS_AND_CONFIG_DB = {
    "drivername": "postgressql",
    "username": "postgres",
    "password": "example",
    "database": "postgres",
    "host": "localhost",
    "port": 5432,
}

# RabbitMQ configuration
QUEUE_NAME = "jobs"
