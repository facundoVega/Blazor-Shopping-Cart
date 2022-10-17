using ShopOnline.Models.Dtos;

namespace ShopOnline.Web.Services.Contractcs
{
    public interface IManageProductsLocalStorageService
    {
        Task<IEnumerable<ProductDto>> GetCollection();
        Task RemoveCollection();

    }
}
