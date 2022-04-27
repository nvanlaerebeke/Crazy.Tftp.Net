FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./src ./

RUN dotnet restore 
RUN dotnet publish -c Release -o /build

# Build runtime image
FROM  mcr.microsoft.com/dotnet/runtime-deps:6.0
WORKDIR /app
COPY --from=build-env /build .
ENTRYPOINT ["Crazy.Tftp"]
