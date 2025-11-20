# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj từ thư mục hiện tại (src/src/)
COPY *.csproj .  

# Restore các package
RUN dotnet restore

# Copy toàn bộ source code trong thư mục hiện tại
COPY . .

# Build và publish ra thư mục out
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy file publish từ stage build
COPY --from=build /app/out .

# Mở port 80
EXPOSE 80

# Chạy API
ENTRYPOINT ["dotnet", "SmartResourceBookingBE.dll"]
