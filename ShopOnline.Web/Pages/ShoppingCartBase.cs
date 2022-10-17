using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contractcs;

namespace ShopOnline.Web.Pages
{
    public class ShoppingCartBase : ComponentBase
    {
        [Inject]
        public IJSRuntime Js { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }
        [Inject]
        public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }

        public List<CartItemDto> ShoppinCartItems { get; set; }
        public string ErrorMessage { get; set; }

        protected string TotalPrice { get; set; }
        protected int TotalQuantity { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                ShoppinCartItems = await ManageCartItemsLocalStorageService.GetCollection();
                CartChanged();
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
            }
        }
         
        private async Task UpdateItemTotalPrice(CartItemDto cartItemDto)
        {
            var item = GetCartItem(cartItemDto.Id);

            if(item != null)
            {
                item.TotalPrice = cartItemDto.Price * cartItemDto.Qty;
            }
            await ManageCartItemsLocalStorageService.SaveCollection(ShoppinCartItems);
        }
        protected async Task UpdateQtyCartItem_Click(int id, int Qty)
        {
            try
            {
                if(Qty > 0)
                {
                    var updateItemDto = new CartItemQtyUpdateDto
                    {
                        CartItemId = id,
                        Qty = Qty
                    };

                    var returnedIpdateItemDto = await ShoppingCartService.UpdateQty(updateItemDto);

                    await UpdateItemTotalPrice(returnedIpdateItemDto);

                    CartChanged();
                    
                    await MakeUpdateQtyButtonVisible(id, false);
                }
                else
                {
                    var item = ShoppinCartItems.FirstOrDefault(i => i.Id == id);
                    
                    if(item != null)
                    {
                        item.Qty = 1;
                        item.TotalPrice = item.Price;

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        protected async Task UpdateQty_Input(int id)
        {
            await MakeUpdateQtyButtonVisible(id, true);
        }

        private   Task MakeUpdateQtyButtonVisible(int id, bool visible)
        {
            return Js.InvokeVoidAsync("MakeUpdateQtyButtonVisible", id, visible).AsTask();
        }

        private void CalculateCartSummaryTotals()
        {
            SetTotalPrice();
            SetTotalQuantity();
        }

        private void SetTotalPrice()
        {
            TotalPrice = ShoppinCartItems.Sum(p => p.TotalPrice).ToString("C");
        }
        private void SetTotalQuantity()
        {
            TotalQuantity = ShoppinCartItems.Sum(p => p.Qty);
        }

        protected async Task DeleteCartItem_Click(int id)
        {
            var cartItemDto = await ShoppingCartService.DeleteItem(id);
            RemoveCartItem(id);
            CartChanged();
        }

        private CartItemDto GetCartItem(int id)
        {
            return ShoppinCartItems.FirstOrDefault(i => i.Id == id);
        }

        private async Task RemoveCartItem(int id)
        {
            var cartItemDto = GetCartItem(id);
            ShoppinCartItems.Remove(cartItemDto);
            CalculateCartSummaryTotals();
            await ManageCartItemsLocalStorageService.SaveCollection(ShoppinCartItems);
        }

        private void CartChanged()
        {
            CalculateCartSummaryTotals();
            ShoppingCartService.RaiseEventOnShoppingCartChanged(TotalQuantity);
        }


    }
}
