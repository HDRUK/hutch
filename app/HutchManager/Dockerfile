FROM node:lts-alpine as spa_build
RUN npm install -g pnpm@7.1.0
WORKDIR /manager-frontend
COPY ./app/manager-frontend .
COPY pnpm-lock.yaml .
COPY pnpm-workspace.yaml .
RUN pnpm install --frozen-lockfile --filter manager-frontend
RUN pnpm run build

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./app/HutchManager ./HutchManager
RUN dotnet restore "HutchManager/HutchManager.csproj"
WORKDIR "/src/HutchManager"
RUN dotnet build "HutchManager.csproj" -c Release -o /app/build

FROM build AS publish
COPY --from=spa_build /manager-frontend ../manager-frontend
RUN dotnet publish "HutchManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HutchManager.dll"]
