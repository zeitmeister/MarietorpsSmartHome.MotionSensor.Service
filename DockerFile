FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
COPY /published/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "NetCore.Docker.dll"]

