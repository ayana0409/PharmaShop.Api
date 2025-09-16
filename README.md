# 💊 PharmaShop.Api

PharmaShop.Api là backend API cho hệ thống thương mại điện tử chuyên cung cấp các sản phẩm y tế như thuốc, thực phẩm chức năng và dụng cụ y tế. Dự án được xây dựng với ASP.NET Core và Entity Framework, hỗ trợ đăng nhập bằng Google, quản lý sản phẩm, giỏ hàng, đơn hàng và người dùng.

## 🌐 API Endpoint

- Base URL: `https://pharmashop-api.onrender.com`
- Ví dụ: `GET /api/shop/homeProduct` – Lấy danh sách sản phẩm hiển thị trang chủ

## 🚀 Tính năng chính

- ✅ Đăng nhập bằng Google OAuth2
- 🛒 Quản lý giỏ hàng và đơn hàng
- 📦 Quản lý sản phẩm, tồn kho, hình ảnh
- 🧑‍⚕️ Phân loại người dùng theo loại (UserType)
- 📂 API RESTful chuẩn hóa cho frontend sử dụng

## 🧰 Công nghệ sử dụng

| Thành phần        | Công nghệ           |
|------------------|---------------------|
| Ngôn ngữ          | C# (.NET 8)         |
| Framework         | ASP.NET Core Web API |
| ORM               | Entity Framework Core (MySQL) |
| Authentication    | ASP.NET Identity + Google OAuth |
| Hosting           | Render.com          |

## 📦 Cài đặt & chạy dự án

```bash
# Clone repository
git clone https://github.com/ayana0409/PharmaShop.Api.git
cd PharmaShop.Api

# Cấu hình chuỗi kết nối trong appsettings.json

# Chạy ứng dụng
dotnet run
