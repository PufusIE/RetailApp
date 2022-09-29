using Caliburn.Micro;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Helpers;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private readonly IProductEndPoint _productEndPoint;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IConfigHelper _configHelper;

        public SalesViewModel(IProductEndPoint productEndPoint, ISaleEndpoint saleEndpoint, IConfigHelper configHelper)
        {
            _productEndPoint = productEndPoint;
            _saleEndpoint = saleEndpoint;
            _configHelper = configHelper;
        }

        //Loading products
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndPoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        // Listbox of products
        private BindingList<ProductModel> _products;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            { 
                _products = value; 
                NotifyOfPropertyChange(() => Products); 
            }
        }

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct
        {
            get { return _selectedProduct; }
            set 
            {
                _selectedProduct = value; 
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
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
        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();        

        public BindingList<CartItemModel> Cart 
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
            decimal taxRate = _configHelper.GetTaxRate()/100;

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

        // Check out button
        public void CheckOut()
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

            _saleEndpoint.PostSale(sale);
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
            // Evaluate if selected item is already in the cart
            CartItemModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingItem != null)
            {
                //Add selected item quantity from box to overall quantity in cart (here)
                existingItem.QuantityInCart += ItemQuantity;

                //Hack - There should be a better way to refresh items
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                //Converts from Products model to CartItemModel
                CartItemModel product = new CartItemModel
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
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;

                //Make sure something is selected              

                return output;
            }

        }
    }
}
