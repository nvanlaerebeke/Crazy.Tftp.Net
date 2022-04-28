.PHONY: build run

PROJECT="Crazy.Tftp"
PROJECT_LOWER=$(shell echo $(PROJECT) | tr A-Z a-z)
PWD=$(shell pwd)

PORT:=69
TFTP_ROOT:=$(shell pwd)/data
VERSION:=$(shell cat VERSION | tr --delete '/n')
ARCH:=amd64

clean:
	rm -rf build

build:
	mkdir -p "${PWD}/build/app"
	dotnet restore "${PWD}/src/Crazy.Tftp" && \
	dotnet publish -c Release -o "${PWD}/build/app" -r linux-x64 --self-contained true -p:PublishTrimmed=true "${PWD}/src/Crazy.Tftp/Crazy.Tftp.csproj"

apt: build
	mkdir -p \
		"${PWD}/build/apt/${PROJECT_LOWER}/DEBIAN" \
		"${PWD}/build/apt/${PROJECT_LOWER}/usr/lib/${PROJECT_LOWER}" \
		"${PWD}/build/apt/${PROJECT_LOWER}/bin" \
		"${PWD}/build/apt/${PROJECT_LOWER}/usr/lib/systemd/system/" \
		"${PWD}/build/output/"

	cp "${PWD}/etc/systemd.service" "${PWD}/build/apt/${PROJECT_LOWER}/usr/lib/systemd/system/crazy.tftp.service"
	cp -R "${PWD}/build/app/"* "${PWD}/build/apt/${PROJECT_LOWER}/usr/lib/${PROJECT_LOWER}"
	cp "${PWD}/etc/postinst" "${PWD}/build/apt/${PROJECT_LOWER}/DEBIAN"

	cat "${PWD}/etc/apt_control.tpl" | PROJECT="${PROJECT}" VERSION="${VERSION}" SIZE=$(shell du -sb "${PWD}/build/apt/${PROJECT_LOWER}/" | awk '{print $$1}') envsubst > "${PWD}/build/apt/${PROJECT_LOWER}/DEBIAN/control"

	rm -f ${PWD}/build/output/*.deb
	dpkg-deb --build "${PWD}/build/apt/${PROJECT_LOWER}" "${PWD}/build/output/${PROJECT_LOWER}-${VERSION}-${ARCH}.deb"

run:
	TFTP_ROOT='${TFTP_ROOT}' PORT='${PORT}' ./build/Crazy.Tftp

container:
	docker build -t "$(PROJECT_LOWER}" -f . 

install:
	cp ./etc/systemd.service /etc/systemd/user/mac.tftp.service
	systemctl daemon-reload
	mkdir -p /opt/Mac.Tftp
	cp -R ./build /opt/Mac.Tftp
	ln -s /opt/Mac.Tftp/Mac.Tftp /bin/Mac.Tftp
