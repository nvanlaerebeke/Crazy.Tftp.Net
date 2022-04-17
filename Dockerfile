#build the dotnet source
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
COPY . /source

RUN cd /source && \
    dotnet restore src/Mac.Tftp && \
    dotnet publish -c Release -o /build -r linux-musl-x64 --self-contained true -p:PublishTrimmed=true src/Mac.Tftp/Mac.Tftp.csproj

#application run container
FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine
LABEL maintainer="Nico van Laerebeke <nico.vanlaerebeke@gmail.com>"

ENV TFTP_ROOT=/var/tftproot                                                                                                       
ENV TFTP_DEFAULT_ROOT=/var/tftpdefault

RUN apk add --no-cache sed

COPY --from=build /build /app

WORKDIR /app
ENTRYPOINT ["/app/Mac.Tftp" ]