using Caliburn.Micro;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWPFDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private readonly IAPIHelper _apiHelper;
        private readonly ILoggedInUser _loggedInUser;

        public SalesViewModel(IAPIHelper apiHelper, ILoggedInUser loggedInUser)
        {
            _apiHelper = apiHelper;
            _loggedInUser = loggedInUser;
        }

        // Listbox of products
        private BindingList<string> _products;

        public BindingList<string> Products
        {
            get { return _products; }
            set 
            { 
                _products = value; 
                NotifyOfPropertyChange(() => Products);
            }
        }

        //How many items to buy
        private int _itemQuantity;

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set 
            { 
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        //Items in cart
        private BindingList<string> _cart;

        public BindingList<string> Cart
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
                // TODO - Replace with math
                return "€ 0.00"; 
            }
            
        }

        public string Tax
        {
            get
            {
                // TODO - Replace with math
                return "€ 0.00";
            }

        }
        public string Total
        {
            get
            {
                // TODO - Replace with math
                return "€ 0.00";
            }

        }

        // Check out button
        public void CheckOut()
        {

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //Make sure something is in the cart

                return output;
            }

        }

        // Add to cart button
        public void AddToCart()
        {

        } 

        public bool CanAddToCart
        {
            get 
            {
                bool output = false;

                //Make sure something is selected
                //there is an item quantity

                return output;
            }
            
        }       

        //Remove from cart button
        public void RemoveFromCart()
        {

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
