using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Enums
{
    public enum AddressType
    {
        Home = 1,           // Nhà riêng
        ConstructionSite = 2, // Công trình (Cần xe tải chuyên dụng)
        Warehouse = 3       // Kho của khách
    }
}
