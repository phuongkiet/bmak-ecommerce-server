# 1. Giai đoạn Build (Dùng SDK nặng để compile code)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj của TẤT CẢ các tầng vào trước để restore (Tận dụng cache của Docker)
# Cấu trúc folder của bạn: Solution nằm ngoài, các folder nằm ngang hàng
COPY ["bmak-ecommerce.API/bmak-ecommerce.API.csproj", "bmak-ecommerce.API/"]
COPY ["bmak-ecommerce.Application/bmak-ecommerce.Application.csproj", "bmak-ecommerce.Application/"]
COPY ["bmak-ecommerce.Domain/bmak-ecommerce.Domain.csproj", "bmak-ecommerce.Domain/"]
COPY ["bmak-ecommerce.Infrastructure/bmak-ecommerce.Infrastructure.csproj", "bmak-ecommerce.Infrastructure/"]

# Lệnh restore các thư viện
RUN dotnet restore "bmak-ecommerce.API/bmak-ecommerce.API.csproj"

# Copy toàn bộ code còn lại vào
COPY . .

# Build code
WORKDIR "/src/bmak-ecommerce.API"
RUN dotnet build "bmak-ecommerce.API.csproj" -c Release -o /app/build

# Publish ra file chạy (DLL)
FROM build AS publish
RUN dotnet publish "bmak-ecommerce.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Giai đoạn Run (Dùng Runtime nhẹ để chạy, không chứa bộ SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bmak-ecommerce.API.dll"]