# teamspeakRconExporter
An Prometheus exporter for the Teamspeak3-Server

This Tools allows you to export metrics from your Teamspeak server to a Prometheus server.

## Build
You need to run the Dockerfile with the root folder as context.
```bash
docker build . -f "TeamspeakRconExporter/Dockerfile" -t "mrh4287/teamspeak_exporter"
```

## Setup
```bash
docker run -p 8123:5000 --name teamspeakExporter mrh4287/teamspeak_exporter hostname=my.server.net username=apiUser password=apiPassword
```

This would start a new container with the name `teamspeakExporter`, which checks the Teamspeak3-Server at `my.server.net`.
It uses the User `apiUser` and the password `apiPassword`.

You can access the metrics at `http://localhost:8123/metrics`.

# Licence
MIT Licence
