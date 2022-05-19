from sqlalchemy import create_engine, Column, Integer, String, DateTime, Text
from sqlalchemy.engine import URL
from sqlalchemy.ext.declarative import declarative_base
import datetime
import argparse

Base = declarative_base()


class Log(Base):
    __tablename__ = "LinkLiteLog"
    id = Column("Id", Integer, primary_key=True, autoincrement=True)
    message = Column("Message", Text, nullable=True)
    message_template = Column("MessageTemplate", Text, nullable=True)
    level = Column("Level", String(128), nullable=True)
    timestamp = Column(
        "TimeStamp", DateTime, nullable=False, default=datetime.datetime.now
    )
    exception = Column("Exception", Text, nullable=True)
    properties = Column("Properties", Text, nullable=True)


def create_log_table():
    parser = argparse.ArgumentParser()
    # Optional command line args
    parser.add_argument(
        "-n",
        "--port",
        dest="port",
        help="The port number for the database.",
        default=None,
        type=int,
    )
    parser.add_argument(
        "--host",
        dest="host",
        help="The host for the database.",
        default="localhost",
    )
    # Required command line args
    required_args = parser.add_argument_group("required arguments")
    required_args.add_argument(
        "--drivername",
        dest="drivername",
        help="""The driver for the database.
        Available drivers:
        - postgresql+psycopg2
        """,
        required=True,
    )
    required_args.add_argument(
        "-u",
        "--username",
        dest="username",
        help="The username for the database.",
        required=True,
    )
    required_args.add_argument(
        "-p",
        "--password",
        dest="password",
        help="The password for the database.",
        required=True,
    )
    required_args.add_argument(
        "-d",
        "--database",
        dest="database",
        help="The name for the database.",
        required=True,
    )
    # parse args
    args = parser.parse_args()

    url = URL(
        drivername=args.drivername,
        username=args.username,
        password=args.password,
        host=args.host,
        port=args.port,
        database=args.database,
    )
    engine = create_engine(url=url)
    Log.metadata.create_all(engine, checkfirst=True)
