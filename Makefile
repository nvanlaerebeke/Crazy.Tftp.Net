.PHONY: build run

PROJECT="Crazy.Tftp"
PROJECT_LOWER=$(shell echo $(PROJECT) | tr A-Z a-z)
PWD=$(shell pwd)

VERSION:=$(shell cat VERSION | tr --delete '/n')
ARCH:=amd64

DATADIR:="${PWD}/data"

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

container:
	docker build -t "${PROJECT_LOWER}:${VERSION}" .

run: container
	docker run -ti --rm \
		--name "${PROJECT_LOWER}" \
		--network=host \
		-v "${DATADIR}:/var/lib/crazy.tftp/data" \
		-v "${PWD}/etc/crazy.tftp_sample_config:/etc/crazy.tftp" \
		${PROJECT_LOWER}:${VERSION}