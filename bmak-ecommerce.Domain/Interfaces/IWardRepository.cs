using bmak_ecommerce.Domain.Entities.Directory;
using bmak_ecommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bmak_ecommerce.Domain.Interfaces
{
    public interface IWardRepository
    {
        Task<PagedList<Ward>> GetWardsByProvinceAsync(WardSpecParams param);
    }
}
