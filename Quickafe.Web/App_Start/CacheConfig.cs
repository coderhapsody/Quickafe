using Autofac;
using Quickafe.DataAccess;
using Quickafe.Providers;
using Quickafe.Providers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quickafe.Web
{
    public class CacheConfig : Module
    {
        private readonly string connectionString;

        public CacheConfig(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            IList<RoleAccessCacheModel> roleAccessCacheData = GetRoleAccessCacheData();

            CacheDataModel cacheData = new CacheDataModel(roleAccessCacheData);
            
            builder.RegisterInstance(cacheData).As<CacheDataModel>().SingleInstance();

            base.Load(builder);                        
        }

        public IList<RoleAccessCacheModel> GetRoleAccessCacheData()
        {
            using (var context = new QuickafeDbContext(connectionString))
            {
                var queryRoleAccess = from role in context.Roles
                                      from menu in role.Menus
                                      where menu.NavigationTo != "#"
                                      select new RoleAccessCacheModel
                                      {
                                          RoleId = role.Id,
                                          RoleName = role.Name,
                                          MenuUrl = menu.NavigationTo
                                      };

                return queryRoleAccess.ToList();
            }
        }
    }
}