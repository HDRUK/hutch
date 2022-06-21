# Logging configuration
DB_LOGGER_NAME = "db"
BACKUP_LOGGER_NAME = "backup"
LOGS_AND_CONFIG_DB = {
    "drivername": "postgresql+psycopg2",
    "username": "postgres",
    "password": "example",
    "database": "postgres",
    "host": "localhost",
    "port": 5432,
}
MSG_FORMAT = "%(levelname)s - %(asctime)s - %(message)s"
DATE_FORMAT = "%d-%b-%y %H:%M:%S"
LOG_TABLE_NAME = "Logs"

# RabbitMQ configuration
QUEUE_NAME = "jobs"

# Manager related configuration
MANAGER_URL = "/path/to/manager"
