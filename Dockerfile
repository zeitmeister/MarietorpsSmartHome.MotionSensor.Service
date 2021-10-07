FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY published/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "MarietorpsSmartHome.MotionSensor.Service.dll"]