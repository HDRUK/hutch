FROM python:3.9.12-buster
WORKDIR /HutchAgent
COPY . .
RUN pip install --upgrade pip
RUN pip install poetry==1.1.14
RUN poetry install --no-dev
EXPOSE 3030
CMD ["poetry", "run", "hutchagent"]
