#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["UserMicroservice/UserMicroservice.csproj", "UserMicroservice/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Models/Models.csproj", "Models/"]
RUN dotnet restore "UserMicroservice/UserMicroservice.csproj"
COPY . .
WORKDIR "/src/UserMicroservice"
RUN dotnet build "UserMicroservice.csproj" -c Release -o /app/build
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh

#FROM build AS publish
#RUN dotnet publish "UserMicroservice.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "UserMicroservice.dll"]

