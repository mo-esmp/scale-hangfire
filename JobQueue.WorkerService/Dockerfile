#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["JobQueue.WorkerService/JobQueue.WorkerService.csproj", "JobQueue.WorkerService/"]
COPY ["JobQueue.Shared/JobQueue.Shared.csproj", "JobQueue.Shared/"]
RUN dotnet restore "JobQueue.WorkerService/JobQueue.WorkerService.csproj"
COPY . .
WORKDIR "/src/JobQueue.WorkerService"
RUN dotnet build "JobQueue.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobQueue.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobQueue.WorkerService.dll"]