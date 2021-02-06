using Quickafe.DataAccess;
using Quickafe.Framework.Base;
using Quickafe.Providers.ViewModels.Role;
using Quickafe.Providers.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using Quickafe.Providers.ViewModels.RoleAccess;
using Quickafe.Resources;

namespace Quickafe.Providers
{
    public class SecurityProvider : BaseProvider
    {
        public SecurityProvider(IQuickafeDbContext context) : base(context)
        {
        }

        public void AddRole(Role role)
        {
            if (DataContext.Roles.Any(r => r.Name == role.Name))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, role.Name));

            DataContext.Roles.Add(role);
            SetAuditFields(role);
            DataContext.SaveChanges();
        }

        public void UpdateRole(Role role)
        {
            if (DataContext.Roles.Any(r => r.Name == role.Name && r.Id != role.Id))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, role.Name));

            SetAuditFields(role);
            DataContext.SaveChanges();
        }

        public IEnumerable<Role> GetRoles()
        {
            return DataContext.Roles.ToList();
        }

        public IQueryable<ListRoleViewModel> List()
        {
            var query = from role in DataContext.Roles
                        orderby role.ChangedWhen descending
                        select new ListRoleViewModel
                        {
                            Id = role.Id,
                            Name = role.Name
                        };
            return query;
        }


        public Role GetRole(long id)
        {
            return DataContext.Roles.SingleOrDefault(cat => cat.Id == id);
        }

        public void DeleteRole(long id)
        {
            var role = GetRole(id);
            if (role != null)
            {
                DataContext.Roles.Remove(role);
                DataContext.SaveChanges();
            }
        }

        public void UpdateRoleAccess(long menuId, IEnumerable<long> roles)
        {
            Menu menu = DataContext.Menus.Single(m => m.Id == menuId);
            menu.Roles.Clear();
            foreach (int roleId in roles)
            {
                Role role = DataContext.Roles.Single(r => r.Id == roleId);
                menu.Roles.Add(role);
            }
            DataContext.SaveChanges();
        }


        public bool ValidateUser(string userName, string password)
        {
            UserLogin user = DataContext.UserLogins.SingleOrDefault(p => p.UserName == userName && p.Password == password && p.IsActive);
            return user != null;
        }

        public void ChangePassword(string userName, string newPassword)
        {
            var user = DataContext.UserLogins.SingleOrDefault(p => p.UserName == userName);
            if (user != null)
            {
                user.Password = newPassword;
                user.LastChangePassword = DateTime.Now;
                SetAuditFields(user);
                DataContext.SaveChanges();
            }
        }

        public UserLogin LogInUser(string userName)
        {
            var user = DataContext.UserLogins.SingleOrDefault(p => p.UserName == userName);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                DataContext.SaveChanges();
            }
            return user;
        }

        #region UserLogin
        public void AddUserLogin(UserLogin userLogin)
        {
            if (DataContext.UserLogins.Any(login => login.UserName == userLogin.UserName))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, userLogin.UserName));

            DataContext.UserLogins.Add(userLogin);
            SetAuditFields(userLogin);

            DataContext.SaveChanges();
        }

        public void UpdateUserLogin(UserLogin userLogin)
        {
            if(DataContext.UserLogins.Any(login => login.UserName == userLogin.UserName && login.Id != userLogin.Id))
                throw new QuickafeException(String.Format(Messages.DuplicateIdentifier, userLogin.UserName));

            SetAuditFields(userLogin);
            DataContext.SaveChanges();
        }

        public void DeleteUserLogin(int userLoginId)
        {
            var selectedUserLogin = DataContext.UserLogins.SingleOrDefault(userLogin => userLogin.Id == userLoginId);
            if (selectedUserLogin != null)
            {
                DataContext.UserLogins.Remove(selectedUserLogin);
                DataContext.SaveChanges();
            }
        }

        public IQueryable<ListUserLoginViewModel> ListUserLogins()
        {
            var query = from userLogin in DataContext.UserLogins
                        join role in DataContext.Roles on userLogin.RoleId equals role.Id
                        orderby userLogin.ChangedWhen descending
                        select new ListUserLoginViewModel
                        {
                            Id = userLogin.Id,
                            UserName = userLogin.UserName,
                            RoleName = role.Name,
                            LastLogin = userLogin.LastLogin,
                            LastChangePassword = userLogin.LastChangePassword,
                            IsActive = userLogin.IsActive,
                            AllowPrintReceipt = userLogin.AllowPrintReceipt,
                            AllowVoidOrder = userLogin.AllowVoidOrder,
                            AllowVoidPayment = userLogin.AllowVoidPayment,
                            IsSystemUser = userLogin.IsSystemUser,
                        };
            return query;
        }

        public IEnumerable<UserLogin> GetUserLogins()
        {
            var query = DataContext.UserLogins;
            return query.ToList();
        }

        public UserLogin GetUserLogin(long userLoginId)
        {
            var selectedUserLogin = DataContext.UserLogins.SingleOrDefault(userLogin => userLogin.Id == userLoginId);
            return selectedUserLogin;
        }

        public UserLogin GetUserLogin(string userName)
        {
            var selectedUserLogin = DataContext.UserLogins.SingleOrDefault(userLogin => userLogin.UserName == userName);
            return selectedUserLogin;
        }
        #endregion

        public IEnumerable<RoleAccessViewModel> GetRolesOfMenu(long menuId)
        {
            var query = from role in DataContext.Roles
                        select new RoleAccessViewModel
                        {
                            MenuId = menuId,
                            RoleId = role.Id,
                            RoleName = role.Name,
                            IsAllowed = role.Menus.Any(m => m.Id == menuId)
                        };
            return query.ToList();
        }

        public IEnumerable<ListMenuViewModel> ListMenus(long? parentMenuId)
        {
            var userLogin = GetUserLogin(Principal.Identity.Name);

            if (userLogin != null)
            {
                var query = from menu in DataContext.Menus
                            from role in menu.Roles
                            where menu.IsActive && menu.ParentMenuId == parentMenuId && role.Id == userLogin.RoleId
                            orderby menu.Seq
                            select new ListMenuViewModel
                            {
                                id = menu.Id,
                                FAIcon = menu.FaIcon,
                                Name = menu.Title,
                                Url = menu.NavigationTo,
                                hasChildren = menu.Menus.Any()
                            };
                return query.ToList();
            }
            return null;
        }

        public IEnumerable<ListMenuViewModel> ListAllMenus(long? parentMenuId)
        {
            var query = from menu in DataContext.Menus
                        where menu.IsActive && menu.ParentMenuId == parentMenuId
                        orderby menu.Seq
                        select new ListMenuViewModel
                        {
                            id = menu.Id,
                            Name = menu.Title,
                            Url = menu.NavigationTo,
                            hasChildren = menu.Menus.Any()
                        };
            return query.ToList();
        }
    }
}
