using Caliburn.Micro;
using RAWPFDesktopUI.EventModels;
using RAWPFDesktopUI.Models;
using RAWPFDesktopUILibrary.Api;
using RAWPFDesktopUILibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RAWPFDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IUserEndpoint _userEndpoint;
        private readonly IEventAggregator _events;
        private readonly ILoggedInUser _loggedInUser;        

        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IUserEndpoint userEndpoint, IEventAggregator events, ILoggedInUser loggedInUser)
        {
            _status = status;
            _window = window;
            _userEndpoint = userEndpoint;
            _events = events;
            _loggedInUser = loggedInUser;            
        }

        //All the users that got populated from user controller api endpoint
        private BindingList<UserModel> _users;

        public BindingList<UserModel> Users
        {
            get { return _users; }
            set 
            { 
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        //Selected user from the list
        private UserModel _selectedUser;

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set 
            { 
                _selectedUser = value;
                SelectedUserName = value.Email;                
                UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());                
                NotifyOfPropertyChange(() => SelectedUser); 
                LoadRoles();
            }
        }

        //Name of selected user
        private string _selectedUserName;

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set 
            {
                _selectedUserName = value; 
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        //Roles that you selected from all user roles, this prop is for Remove Role button
        private string _selectedUserRole;

        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set 
            { 
                _selectedUserRole = value; 
                NotifyOfPropertyChange(() => SelectedUserRole);
                NotifyOfPropertyChange(() => CanRemoveSelectedRole);                
            }
        }
                
        //All the avaliabel roles that got populated from LoadRoles() method
        private BindingList<string> _avaliableRoles = new BindingList<string>();

        public BindingList<string> AvaliableRoles
        {
            get { return _avaliableRoles; }
            set 
            {
                _avaliableRoles = value;
                NotifyOfPropertyChange(() => AvaliableRoles);
            }
        }

        // Selected avaliable role
        private string _selectedAvaliableRole;

        public string SelectedAvaliableRole
        {
            get { return _selectedAvaliableRole; }
            set 
            {
                _selectedAvaliableRole = value; 
                NotifyOfPropertyChange(() => SelectedAvaliableRole);
                NotifyOfPropertyChange(() => CanAddSelectedRole);
            }
        }

        //Roles of selected user
        private BindingList<string> _userRoles = new BindingList<string>();

        public BindingList<string> UserRoles
        {
            get { return _userRoles; }
            set {
                _userRoles = value;
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            //Checking if logged in user have access to this page
            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                var settings = GetPopupSettings();

                var statusInfo = IoC.Get<StatusInfoViewModel>();

                if (ex.Message.ToLower() == "unauthorized")
                {
                    statusInfo.UpdateMessageInfo("Unauthorized Access", "You do not have access to Administration Form.");
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

        //Setting setup for status window
        private dynamic GetPopupSettings()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.Title = "System Error";

            return settings;
        } 

        //Populating list of users
        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();
            Users = new BindingList<UserModel>(userList);
        }

        //Populating List of avaliable roles for selected person 
        public async Task LoadRoles()
        {
            var roles = await _userEndpoint.GetAllRoles();

            AvaliableRoles.Clear();

            foreach (var role in roles)
            {
                if (!UserRoles.Contains(role.Value))
                {
                    AvaliableRoles.Add(role.Value);
                }
            }
        }

        public bool CanAddSelectedRole
        {
            get
            {
                if (SelectedUser is null || SelectedAvaliableRole is null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }        

        //Adding SelectedRole to user
        public async void AddSelectedRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvaliableRole);

            UserRoles.Add(SelectedAvaliableRole);
            AvaliableRoles.Remove(SelectedAvaliableRole);

            await UpdateCurrentUser();
        }

        public bool CanRemoveSelectedRole
        {
            get
            {
                if (SelectedUser is null || SelectedUserRole is null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //removing selected role from user
        public async void RemoveSelectedRole()
        {
            await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

            AvaliableRoles.Add(SelectedUserRole);
            UserRoles.Remove(SelectedUserRole);

            await UpdateCurrentUser();
        }

        public async Task UpdateCurrentUser()
        {
            if (_loggedInUser.EmailAddress == SelectedUser.Email)
            {               
                await _events.PublishOnUIThreadAsync(new OwnRoleUpdateEventModel());

                var settings = GetPopupSettings();

                var statusInfo = IoC.Get<StatusInfoViewModel>();

                statusInfo.UpdateMessageInfo("Own Role Update", "You updated your role, please log in again.");
                _window.ShowDialogAsync(statusInfo, null, settings);
            }
        }
    }
}
