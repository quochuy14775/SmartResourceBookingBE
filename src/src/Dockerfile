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

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variable for ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Chạy API với tên assembly đúng
ENTRYPOINT ["dotnet", "src.dll"]
