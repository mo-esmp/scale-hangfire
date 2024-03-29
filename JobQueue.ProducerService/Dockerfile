#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["JobQueue.ProducerService/JobQueue.ProducerService.csproj", "JobQueue.ProducerService/"]
COPY ["JobQueue.Shared/JobQueue.Shared.csproj", "JobQueue.Shared/"]
RUN dotnet restore "JobQueue.ProducerService/JobQueue.ProducerService.csproj"
COPY . .
WORKDIR "/src/JobQueue.ProducerService"
RUN dotnet build "JobQueue.ProducerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobQueue.ProducerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobQueue.ProducerService.dll"]