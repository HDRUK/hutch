FROM node:lts-alpine as spa_build
WORKDIR /manager-frontend
COPY ../manager-frontend .
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . ./HutchManager
RUN dotnet restore "HutchManager/HutchManager.csproj"
WORKDIR "/src/HutchManager"
RUN dotnet build "HutchManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HutchManager.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=spa_build /manager-front-end .
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HutchManager.dll"]
