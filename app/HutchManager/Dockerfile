FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . ./HutchManager
RUN dotnet restore "HutchManager/HutchManager.csproj"
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef database update
WORKDIR "/src/HutchManager"
RUN dotnet build "HutchManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HutchManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HutchManager.dll"]
