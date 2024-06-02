using Accessories_Store.Models.Entities;

namespace Accessories_Store.Areas.Admin.Services
{
    public interface IRatingRepo
    {
        dynamic findAllById(int id);
    }
}
