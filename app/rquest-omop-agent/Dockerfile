FROM python:3.10-buster
COPY app/rquest-omop-agent /app/rquest-omop-agent
COPY lib/hutch_utils /lib/hutch_utils
COPY lib/omop_entities /lib/omop_entities
COPY lib/rquest_dto /lib/rquest_dto
WORKDIR /app/rquest-omop-agent
RUN pip install --no-cache-dir .
