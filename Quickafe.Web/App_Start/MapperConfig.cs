using Autofac;
using AutoMapper;
using Quickafe.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quickafe.Web
{
    internal class MapperConfig : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(Configure);
            IMapper mapper = config.CreateMapper();

            builder.RegisterInstance(mapper).SingleInstance();

            base.Load(builder);
        }

        private void Configure(IMapperConfigurationExpression config)
        {            
            config.CreateMap<ProductCategory, Providers.ViewModels.ProductCategory.CreateEditViewModel>().ReverseMap();
            config.CreateMap<PaymentType, Providers.ViewModels.PaymentType.CreateEditViewModel>().ReverseMap();
            config.CreateMap<Product, Providers.ViewModels.Product.CreateEditViewModel>().ReverseMap();
            config.CreateMap<Table, Providers.ViewModels.Table.CreateEditViewModel>().ReverseMap();

            #region Sales
            config.CreateMap<Order, Providers.Sales.ViewModels.Order.CreateEditViewModel>()
                .ReverseMap();
            config.CreateMap<OrderDetail, Providers.Sales.ViewModels.Order.OrderDetailEntryViewModel>()
                .ForMember(m => m.DetailDiscPercent, opt => opt.MapFrom(m => m.DiscPercent))
                .ForMember(m => m.DetailDiscValue, opt => opt.MapFrom(m => m.DiscValue))
                .ForMember(m => m.ProductName, opt => opt.MapFrom(m => m.Product.Name))
                .ForMember(m => m.ProductCode, opt => opt.MapFrom(m => m.Product.Code))
                .ForMember(m => m.ProductCategoryName, opt => opt.MapFrom(m => m.Product.ProductCategory.Name))
                .ReverseMap();
            config.CreateMap<Providers.Sales.ViewModels.Order.OrderDetailEntryViewModel, OrderDetail>()
                .ForMember(m => m.DiscPercent, opt => opt.MapFrom(m => m.DetailDiscPercent))
                .ForMember(m => m.DiscValue, opt => opt.MapFrom(m => m.DetailDiscValue));
            config.CreateMap<Order, Providers.Sales.ViewModels.Order.DetailViewModel>()
                .ForMember(m => m.TableCode, opt => opt.MapFrom(m => m.Table.Code))
                .ForMember(m => m.OrderDate, opt => opt.MapFrom(m => m.Date));
            config.CreateMap<PaymentDetail, Providers.Sales.ViewModels.Payment.PaymentDetailViewModel>()
                .ReverseMap();
            config.CreateMap<Payment, Providers.Sales.ViewModels.Payment.DetailViewModel>()
                .ForMember(m => m.PaymentDate, opt => opt.MapFrom(m => m.Date))
                .ForMember(m => m.OrderDate, opt => opt.MapFrom(m => m.Order.Date))
                .ForMember(m => m.OrderNo, opt => opt.MapFrom(m => m.Order.OrderNo))
                .ForMember(m => m.OrderType, opt => opt.MapFrom(m => m.Order.OrderType))
                .ReverseMap();
            #endregion

            #region Security
            config.CreateMap<UserLogin, Providers.ViewModels.User.CreateEditViewModel>().ReverseMap();
            config.CreateMap<Role, Providers.ViewModels.Role.CreateEditViewModel>().ReverseMap();
            #endregion

            #region Invetory In
            config.CreateMap<Inventory, Providers.Inventory.ViewModels.InventoryIn.CreateEditViewModel>().ReverseMap();
            config.CreateMap<InventoryDetail, Providers.Inventory.ViewModels.InventoryIn.InventoryDetailEntryViewModel>()
                .ForMember(m => m.ProductName, opt => opt.MapFrom(m => m.Product.Name))
                .ForMember(m => m.ProductCode, opt => opt.MapFrom(m => m.Product.Code))
                .ReverseMap();
            #endregion

            #region Invetory Out
            config.CreateMap<Inventory, Providers.Inventory.ViewModels.InventoryOut.CreateEditViewModel>().ReverseMap();
            config.CreateMap<InventoryDetail, Providers.Inventory.ViewModels.InventoryOut.InventoryDetailEntryViewModel>()
                .ForMember(m => m.ProductName, opt => opt.MapFrom(m => m.Product.Name))
                .ForMember(m => m.ProductCode, opt => opt.MapFrom(m => m.Product.Code))
                .ReverseMap();
            #endregion
        }
    }
}