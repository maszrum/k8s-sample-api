FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["K8s.SampleApi.csproj", "K8s.SampleApi/"]
RUN dotnet restore "K8s.SampleApi/K8s.SampleApi.csproj"
COPY . ./K8s.SampleApi/
WORKDIR "/src/K8s.SampleApi"
RUN dotnet build "K8s.SampleApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "K8s.SampleApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8s.SampleApi.dll"]
