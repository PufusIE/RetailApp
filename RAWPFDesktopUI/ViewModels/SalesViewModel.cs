using AutoMapper;
using Caliburn.Micro;
using RAWPFDesktopUI.Models;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Helpers;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RAWPFDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private readonly IProductEndPoint _productEndPoint;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IMapper _mapper;
        private readonly IConfigHelper _configHelper;
        private readonly IWindowManager _window;

        public SalesViewModel(IProductEndPoint productEndPoint, ISaleEndpoint saleEndpoint,
            IMapper mapper, IConfigHelper configHelper, IWindowManager window)
        {
            _productEndPoint = productEndPoint;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _configHelper = configHelper;
            _window = window;
        }

        //Waiting until view loads before loading products
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            //Checking if logged in user have access to this page
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                //Setting setup for status window
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                var statusInfo = IoC.Get<StatusInfoViewModel>();

                if (ex.Message.ToLower() == "unauthorized")
                {
                    statusInfo.UpdateMessageInfo("Unauthorized Access", "You do not have access to Sale Form.");
                    _window.ShowDialogAsync(statusInfo, null, settings);
                }
                else
                {
                    statusInfo.UpdateMessageInfo("Fatal Exception", ex.Message);
                    _window.ShowDialogAsync(statusInfo, null, settings);
                }

                await TryCloseAsync();
            }
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            
            //Mapping models with automapper
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        // Listbox of products
        private BindingList<ProductDisplayModel> _products;

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set 
            { 
                _products = value; 
                NotifyOfPropertyChange(() => Products); 
            }
        }

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            {
                _selectedProduct = value; 
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set 
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        //How many items to buy
        private int _itemQuantity = 1;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        //Items in cart
        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();        

        public BindingList<CartItemDisplayModel> Cart 
        {
            get { return _cart; }
            set 
            { 
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public string SubTotal
        {
            get 
            {
                decimal subTotal = CalculateSubTotal();               
                
                return subTotal.ToString("C", CultureInfo.GetCultureInfo("en-IE"));
            }            
        }

        public decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += (item.Product.RetailPrice * item.QuantityInCart);
            }

            return subTotal;
        }

        public decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate();

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x=> x.Product.RetailPrice * x.QuantityInCart * taxRate);
                     
            return taxAmount;
        }

        public string Tax
        {
            get
            {
                decimal taxAmount = CalculateTax();

                return taxAmount.ToString("C", CultureInfo.GetCultureInfo("en-IE"));
            }
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();
                
                return total.ToString("C", CultureInfo.GetCultureInfo("en-IE"));
            }
        }

        private async Task ResetSalesViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();

            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        // Check out button
        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);
            await ResetSalesViewModel();
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //Make sure something is in the cart
                if (Cart.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        // Add to cart button
        public void AddToCart()
        {
            // Check if selected item is already in the cart
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingItem != null)
            {
                //Add selected item quantity from box to overall quantity in cart (here)
                existingItem.QuantityInCart += ItemQuantity;
            }
            else
            {
                //Converts from Products model to CartItemDisplayModel
                CartItemDisplayModel product = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };

                Cart.Add(product);
            }

            //Removes items from total stock so you can't add more than you have in stock
            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        } 

        public bool CanAddToCart
        {
            get 
            {
                bool output = false;
                
                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }

                return output;
            }
        }       

        //Remove from cart button
        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock++;

            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart--;
            }
            else
            { 
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                if (SelectedCartItem != null && SelectedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }

                return output;
            }
        }
    }
}
