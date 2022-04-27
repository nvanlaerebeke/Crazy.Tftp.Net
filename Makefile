.PHONY: build run

PROJECT="Mac.Tftp.Net"
PWD=$(shell pwd)
PORT:=69
TFTP_ROOT:=$(shell pwd)/data

clean:
	rm -rf build

build:
	dotnet restore src/Crazy.Tftp && \
	dotnet publish -c Release -o ./build -r linux-x64 --self-contained true -p:PublishTrimmed=true src/Crazy.Tftp/Crazy.Tftp.csproj

run:
	TFTP_ROOT='${TFTP_ROOT}' PORT='${PORT}' ./build/Crazy.Tftp

install:
	cp ./etc/systemd.service /etc/systemd/user/mac.tftp.service
	systemctl daemon-reload
	mkdir -p /opt/Mac.Tftp
	cp -R ./build /opt/Mac.Tftp
	ln -s /opt/Mac.Tftp/Mac.Tftp /bin/Mac.Tftp
