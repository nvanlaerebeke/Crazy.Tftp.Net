# Crazy.Tftp.Net

TFTP server using [Tftp.NET](https://github.com/Callisto82/tftp.net) with additional filter options.  
Has the ability to send requests from a specific `IP` or Mac address to it's own directory.  

## Requirements

### Building on the Host

To build on the host the `.net 6.0 SDK` is required.  

### Building the docker container

When building using docker the build will be done during the docker build so only docker is required.

### Apt package

To build the apt package, `dpkg-deb` must be installed.

## Configuration

To configure the `TFTP` server add a configuration file at `/etc/crazy.tftp`.  
An example file:

```text
{
  "data_location": "",
  "port": 69,
  "mac_filter": [
    "0e:ee:d3:7d:4a:b9",
    "08:84:a8:58:e0:5c",
    "95:2d:74:1a:a1:b8",
    "f7:19:19:67:7f:67",
    "9f:eb:3e:d0:2d:6a"
  ],
  "ip_filter": [
    "192.168.0.100",
    "192.168.0.101"
  ]
}
```

## Docker

To run from docker the included `Makefile` can be used or manually run the `docker build` and run.  

Using the `Makefile`:

```console
make run
```

This will build the image and launch it with:

- DATADIR as ./data
- PORT: 69
- Added on host network
- ./etc/crazy.ftp_sample_config as configuration file

Reason to add the docker container on the host network is due to the mac filter.  
If the MAC address filter isn't used the container doesn't have to run on the host network.  

Example without MAC address filter:

```console
    docker build -t crazy_tftp .
    docker run -ti --rm \
        --name crazy_tftp \
        -p 69:69/udp \
        -v `pwd`/data:/var/lib/crazy.tftp/data \
        -v `pwd`/etc/crazy.tftp_sample_config:/etc/crazy.tftp \
        crazy_tftp
```

To run the container on the host network replace `-p 69:69/udp` with `--network host`.

## Docker-compose

A `docker-compose.yaml` file is included, to build and run the `tftp` server, run:

```console
docker-compose -d up
```

This will start on port 69 with the default settings and example configuration file.  
Note that this will run in it's on network so the MAC address filtering will not work.  
To get this to work modify the compose file to run the container on the host network.

## Bare Metal

The `Makefile` gives you the ability to create an `apt` package, to generate it, run:

```console
make apt
```

The resulting apt package will be located in `./build/output`

## Custom Build

To build the source run the `make build` command:

```console
make build
```

The binaries and dependencies will be put in `./build/app/`

## Running from Visual Studio (Code)

The application was written to run on Linux or at least a Linux container.  

Open up the solution and run it.
The paths that will be used by default are:

- Data being served: `/var/lib/crazy.tftp/data`
- Optional configuration file: `/etc/crazy.tftp`
