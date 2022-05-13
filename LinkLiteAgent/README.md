# linkliteagent

This is the source code for the Link Lite agent. The agent will:
  - read data requests from a queue,
  - find the data in a database,
  - perform obfuscation, low count filtering, etc,
  - place results in a results queue.