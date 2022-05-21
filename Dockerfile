FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./src ./

RUN dotnet restore 
RUN dotnet publish -c Release -o /build -r linux-x64 --self-contained true -p:PublishTrimmed=true /p:DebugSymbols=false /p:DebugType=None "Crazy.Tftp/Crazy.Tftp.csproj"

# Build runtime image
FROM  mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app

RUN apt-get update && \
    apt-get install -y net-tools && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build-env /build .
ENTRYPOINT ["/app/Crazy.Tftp"]
