#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Install dependencies for PuppeteerSharp (Chromium)
#RUN apt-get update \
    #&& apt-get install -y wget gnupg \
    #&& wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    #&& sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    #&& apt-get update \
    #&& apt-get install -y google-chrome-stable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf libxss1 \
      #--no-install-recommends \
    #&& rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ComicShelf/ComicShelf.csproj", "ComicShelf/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Backend/Backend.csproj", "Backend/"]
RUN dotnet restore "./ComicShelf/ComicShelf.csproj"
COPY . .
WORKDIR "/src/ComicShelf"
RUN dotnet build "./ComicShelf.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ComicShelf.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ComicShelf.dll"]