FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/PowerShellPayloadGenerator/PowerShellPayloadGenerator.csproj", "src/PowerShellPayloadGenerator/"]
RUN dotnet restore "src/PowerShellPayloadGenerator/PowerShellPayloadGenerator.csproj"
COPY . .
RUN dotnet build "src/PowerShellPayloadGenerator/PowerShellPayloadGenerator.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "PowerShellPayloadGenerator.dll"]
