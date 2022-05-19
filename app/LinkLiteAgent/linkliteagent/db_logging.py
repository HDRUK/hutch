from sqlalchemy import create_engine, Column, Integer, String, DateTime, Text
from sqlalchemy.engine import URL
from sqlalchemy.ext.declarative import declarative_base
import datetime


Base = declarative_base()


class Log(Base):
    __tablename__ = "LinkLiteLog"
    id = Column(Integer, primary_key=True, autoincrement=True)
    message = Column("Message", Text, nullable=True)
    message_template = Column("MessageTemplate", Text, nullable=True)
    level = Column("Level", String(128), nullable=True)
    timestamp = Column(
        "TimeStamp", DateTime, nullable=False, default=datetime.datetime.now
    )
    exception = Column("Exception", Text, nullable=True)
    properties = Column("Properties", Text, nullable=True)


def create_log_table():
    url = URL(
        drivername="postgresql+psycopg2",
        username="postgres",
        password="example",
        host="localhost",
        port=5432,
        database="postgres",
    )
    engine = create_engine(url=url)
    Log.metadata.create_all(engine, checkfirst=True)
