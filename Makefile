.PHONY: build run

PROJECT="Mac.Tftp.Net"
PWD=$(shell pwd)
PORT:=69
MAC_ADDRESSES:= "[ ]"
TFTP_ROOT:=$(shell pwd)/data

build:
	dotnet restore src/Mac.Tftp && \
	dotnet publish -c Release -o ./build -r linux-x64 --self-contained true -p:PublishTrimmed=true src/Mac.Tftp/Mac.Tftp.csproj

run:
	MAC_ADDRESSES='${MAC_ADDRESSES}' TFTP_ROOT='${TFTP_ROOT}' PORT='${PORT}' ./build/Mac.Tftp

install:
	cp ./etc/systemd.service /etc/systemd/user/mac.tftp.service
	systemctl daemon-reload
	mkdir -p /opt/Mac.Tftp
	cp -R ./build /opt/Mac.Tftp
	ln -s /opt/Mac.Tftp/Mac.Tftp /bin/Mac.Tftp