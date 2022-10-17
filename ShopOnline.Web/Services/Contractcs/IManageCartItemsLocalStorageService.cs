

using ShopOnline.Models.Dtos;

namespace ShopOnline.Web.Services.Contractcs
{
    public interface IManageCartItemsLocalStorageService
    {
        Task<List<CartItemDto>> GetCollection();
        Task SaveCollection(List<CartItemDto> cartItemsDto);
        Task RemoveCollection();
    }
}
