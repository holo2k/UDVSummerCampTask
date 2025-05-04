FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY UDVSummerCampTask/UDVSummerCampTask.csproj UDVSummerCampTask/
RUN dotnet restore UDVSummerCampTask/UDVSummerCampTask.csproj

COPY . .

RUN dotnet publish UDVSummerCampTask/UDVSummerCampTask.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "UDVSummerCampTask.dll"]
