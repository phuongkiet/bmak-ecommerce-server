# Test Data Mẫu cho API

## 1. Test Create Product - Đầy đủ các trường (có Tags)

```json
POST /api/products
{
  "name": "Gạch ốp tường Monocolor 60x60",
  "sku": "GACH-MONO-60X60-001",
  "basePrice": 150000,
  "salePrice": 135000,
  "salesUnit": "Thùng",
  "priceUnit": "m²",
  "conversionFactor": 1.44,
  "categoryId": 2,
  "weight": 25.5,
  "imageUrl": "https://example.com/images/gach-mono-60x60.jpg",
  "specificationsJson": "{\"material\":\"ceramic\",\"origin\":\"Vietnam\",\"thickness\":\"8mm\",\"waterAbsorption\":\"<3%\"}",
  "isActive": true,
  "attributes": [
    {
      "attributeId": 1,
      "value": "60x60"
    },
    {
      "attributeId": 2,
      "value": "Xám"
    },
    {
      "attributeId": 3,
      "value": "Matt"
    }
  ],
  "tagIds": [1, 2, 3]
}
```
**Lưu ý về IDs:**
- `categoryId: 2` = "Gạch ốp tường"
- `attributeId: 1` = "Kích thước" (SIZE)
- `attributeId: 2` = "Màu sắc" (COLOR)
- `attributeId: 3` = "Bề mặt" (SURFACE)
- `tagIds: [1, 2, 3]` = "Bán chạy", "Mới", "Khuyến mãi"

## 2. Test Create Product - Chỉ các trường bắt buộc (các trường optional để null)

```json
POST /api/products
{
  "name": "Gạch lát nền Porcelain 80x80",
  "sku": "GACH-POR-80X80-001",
  "basePrice": 200000,
  "salePrice": 180000,
  "salesUnit": "Thùng",
  "priceUnit": "m²",
  "conversionFactor": 1.6,
  "categoryId": 3
}
```
→ Các trường optional sẽ được set default: `Weight = 0`, `IsActive = true`, `SpecificationsJson = "{}"`

## 2b. Test Create Product - Với Tags nhưng không có Attributes

```json
POST /api/products
{
  "name": "Gạch lát nền Porcelain 80x80",
  "sku": "GACH-POR-80X80-002",
  "basePrice": 200000,
  "salePrice": 180000,
  "salesUnit": "Thùng",
  "priceUnit": "m²",
  "conversionFactor": 1.6,
  "categoryId": 3,
  "tagIds": [1, 4]
}
```
→ `tagIds: [1, 4]` = "Bán chạy", "Cao cấp"

## 3. Test Create Product - Có ImageUrl và Tags nhưng không có SpecificationsJson

```json
POST /api/products
{
  "name": "Xi măng PC40",
  "sku": "XM-PC40-50KG",
  "basePrice": 85000,
  "salePrice": 80000,
  "salesUnit": "Bao",
  "priceUnit": "Bao",
  "conversionFactor": 1.0,
  "categoryId": 4,
  "imageUrl": "https://example.com/images/xi-mang-pc40.jpg",
  "weight": 50.0,
  "tagIds": [2, 5]
}
```
→ `SpecificationsJson` sẽ được tự động tạo: `{"imageUrl":"https://example.com/images/xi-mang-pc40.jpg"}`
→ `tagIds: [2, 5]` = "Mới", "Giá tốt"

## 3b. Test Create Product - Có ImageUrl nhưng không có SpecificationsJson (không tags)

```json
POST /api/products
{
  "name": "Xi măng PC40",
  "sku": "XM-PC40-50KG",
  "basePrice": 85000,
  "salePrice": 80000,
  "salesUnit": "Bao",
  "priceUnit": "Bao",
  "conversionFactor": 1.0,
  "categoryId": 4,
  "imageUrl": "https://example.com/images/xi-mang-pc40.jpg",
  "weight": 50.0
}
```
→ `SpecificationsJson` sẽ được tự động tạo: `{"imageUrl":"https://example.com/images/xi-mang-pc40.jpg"}`

## 4. Test Create Product - Với Attributes và Tags

```json
POST /api/products
{
  "name": "Gạch ốp tường 3D màu trắng",
  "sku": "GACH-3D-WHITE-001",
  "basePrice": 180000,
  "salePrice": 160000,
  "salesUnit": "Thùng",
  "priceUnit": "m²",
  "conversionFactor": 1.44,
  "categoryId": 2,
  "weight": 28.0,
  "isActive": true,
  "attributes": [
    {
      "attributeId": 1,
      "value": "30x60"
    },
    {
      "attributeId": 2,
      "value": "Trắng"
    },
    {
      "attributeId": 3,
      "value": "3D"
    },
    {
      "attributeId": 4,
      "value": "Ceramic"
    },
    {
      "attributeId": 6,
      "value": "Vietnam"
    }
  ],
  "tagIds": [1, 3, 4]
}
```
→ `tagIds: [1, 3, 4]` = "Bán chạy", "Khuyến mãi", "Cao cấp"

```json
POST /api/products
{
  "name": "Gạch ốp tường 3D màu trắng",
  "sku": "GACH-3D-WHITE-001",
  "basePrice": 180000,
  "salePrice": 160000,
  "salesUnit": "Thùng",
  "priceUnit": "m²",
  "conversionFactor": 1.44,
  "categoryId": 2,
  "weight": 28.0,
  "isActive": true,
  "attributes": [
    {
      "attributeId": 1,
      "value": "30x60"
    },
    {
      "attributeId": 2,
      "value": "Trắng"
    },
    {
      "attributeId": 3,
      "value": "3D"
    },
    {
      "attributeId": 4,
      "value": "Ceramic"
    },
    {
      "attributeId": 6,
      "value": "Vietnam"
    }
  ]
}
```

## 5. Test Create Product - Sắt thép với Tags

```json
POST /api/products
{
  "name": "Thép D10",
  "sku": "THEP-D10-11M",
  "basePrice": 12000,
  "salePrice": 11500,
  "salesUnit": "Cây",
  "priceUnit": "Cây",
  "conversionFactor": 1.0,
  "categoryId": 5,
  "weight": 7.2,
  "specificationsJson": "{\"diameter\":\"10mm\",\"length\":\"11.7m\",\"weightPerUnit\":\"7.21kg\"}",
  "tagIds": [5]
}
```
→ `tagId: 5` = "Giá tốt"

## 6. Test Create Product - Product cao cấp với đầy đủ thông tin

```json
POST /api/products
{
  "name": "Gạch ốp tường Premium 60x60",
  "sku": "GACH-PREM-60X60-001",
  "basePrice": 250000,
  "salePrice": 220000,
  "salesUnit": "Thùng",
  "priceUnit": "m²",
  "conversionFactor": 1.44,
  "categoryId": 2,
  "weight": 30.0,
  "imageUrl": "https://example.com/images/gach-premium-60x60.jpg",
  "specificationsJson": "{\"material\":\"porcelain\",\"origin\":\"Italy\",\"thickness\":\"10mm\",\"waterAbsorption\":\"<0.5%\",\"grade\":\"A+\"}",
  "isActive": true,
  "attributes": [
    {
      "attributeId": 1,
      "value": "60x60"
    },
    {
      "attributeId": 2,
      "value": "Trắng"
    },
    {
      "attributeId": 3,
      "value": "Bóng"
    },
    {
      "attributeId": 4,
      "value": "Porcelain"
    },
    {
      "attributeId": 5,
      "value": "10mm"
    },
    {
      "attributeId": 6,
      "value": "Italy"
    }
  ],
  "tagIds": [1, 4]
}
```
→ `tagIds: [1, 4]` = "Bán chạy", "Cao cấp"

## 7. Test Create Product - Sản phẩm khuyến mãi

```json
POST /api/products
{
  "name": "Gạch lát nền giả gỗ 80x80",
  "sku": "GACH-GO-80X80-001",
  "basePrice": 180000,
  "salePrice": 140000,
  "salesUnit": "Thùng",
  "priceUnit": "m²",
  "conversionFactor": 1.6,
  "categoryId": 3,
  "weight": 26.5,
  "imageUrl": "https://example.com/images/gach-go-80x80.jpg",
  "isActive": true,
  "attributes": [
    {
      "attributeId": 1,
      "value": "80x80"
    },
    {
      "attributeId": 2,
      "value": "Nâu"
    },
    {
      "attributeId": 3,
      "value": "Giả gỗ"
    }
  ],
  "tagIds": [3, 5]
}
```
→ `tagIds: [3, 5]` = "Khuyến mãi", "Giá tốt"

---

## Test Create Category

```json
POST /api/categories
{
  "name": "Gạch nhập khẩu",
  "description": "Gạch nhập khẩu cao cấp từ các nước",
  "parentId": 1
}
```

```json
POST /api/categories
{
  "name": "Gạch Việt Nam",
  "description": "Gạch sản xuất trong nước",
  "parentId": 1
}
```

```json
POST /api/categories
{
  "name": "Gạch cao cấp",
  "description": "Danh mục gốc",
  "parentId": null
}
```

---

## Test Create ProductAttribute

```json
POST /api/productattributes
{
  "name": "Độ bóng",
  "code": "GLOSS"
}
```

```json
POST /api/productattributes
{
  "name": "Chống trơn",
  "code": "ANTI_SLIP"
}
```

---

## Test Create ProductAttributeValue

**Lưu ý:** Cần có ProductId và AttributeId đã tồn tại trong DB.

```json
POST /api/productattributes/values
{
  "value": "80x80",
  "extraData": null,
  "productId": 1,
  "attributeId": 1
}
```

```json
POST /api/productattributes/values
{
  "value": "Đỏ",
  "extraData": "#FF0000",
  "productId": 1,
  "attributeId": 2
}
```

---

## Lưu ý về IDs:

Sau khi seed data, thường các ID sẽ như sau:

**Categories:**
- ID 1: Vật liệu xây dựng
- ID 2: Gạch ốp tường
- ID 3: Gạch lát nền
- ID 4: Xi măng
- ID 5: Sắt thép

**ProductAttributes:**
- ID 1: Kích thước (SIZE)
- ID 2: Màu sắc (COLOR)
- ID 3: Bề mặt (SURFACE)
- ID 4: Chất liệu (MATERIAL)
- ID 5: Độ dày (THICKNESS)
- ID 6: Xuất xứ (ORIGIN)

**Tags:**
- ID 1: Bán chạy
- ID 2: Mới
- ID 3: Khuyến mãi
- ID 4: Cao cấp
- ID 5: Giá tốt

**Để kiểm tra IDs thực tế trong DB, có thể query:**
```sql
SELECT Id, Name FROM Category;
SELECT Id, Name, Code FROM ProductAttribute;
SELECT Id, Name, Slug FROM Tag;
```

---

## cURL Examples:

### Create Product (đầy đủ):
```bash
curl -X POST "https://localhost:7228/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gạch ốp tường Monocolor 60x60",
    "sku": "GACH-MONO-60X60-001",
    "basePrice": 150000,
    "salePrice": 135000,
    "salesUnit": "Thùng",
    "priceUnit": "m²",
    "conversionFactor": 1.44,
    "categoryId": 2,
    "weight": 25.5,
    "imageUrl": "https://example.com/images/gach-mono-60x60.jpg",
    "specificationsJson": "{\"material\":\"ceramic\"}",
    "isActive": true,
    "attributes": [
      {"attributeId": 1, "value": "60x60"},
      {"attributeId": 2, "value": "Xám"}
    ]
  }'
```

### Create Product (tối giản):
```bash
curl -X POST "https://localhost:7228/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gạch lát nền 80x80",
    "sku": "GACH-80X80-001",
    "basePrice": 200000,
    "salePrice": 180000,
    "salesUnit": "Thùng",
    "priceUnit": "m²",
    "conversionFactor": 1.6,
    "categoryId": 3
  }'
```

