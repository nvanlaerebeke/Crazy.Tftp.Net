FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Mac.Tftp/Mac.Tftp.csproj", "Mac.Tftp/"]
RUN dotnet restore "Mac.Tftp/Mac.Tftp.csproj"
COPY . .
WORKDIR "/src/Mac.Tftp"
RUN dotnet build "Mac.Tftp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mac.Tftp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mac.Tftp.dll"]
