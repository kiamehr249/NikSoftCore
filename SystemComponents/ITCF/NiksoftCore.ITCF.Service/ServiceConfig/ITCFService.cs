﻿using Microsoft.Extensions.Configuration;

namespace NiksoftCore.ITCF.Service
{
    public interface IITCFService
    {
        IUserLegalFormService IUserLegalFormServ { get; set; }
        IBusinessService IBusinessServ { get; set; }
        IBusinessCategoryService IBusinessCategoryServ { get; set; }
        IIndustrialParkService IIndustrialParkServ { get; set; }
        ICountryService iCountryServ { get; set; }
        IProvinceService iProvinceServ { get; set; }
        ICityService iCityServ { get; set; }
    }

    public class ITCFService : IITCFService
    {
        public IUserLegalFormService IUserLegalFormServ { get; set; }
        public IBusinessService IBusinessServ { get; set; }
        public IBusinessCategoryService IBusinessCategoryServ { get; set; }
        public IIndustrialParkService IIndustrialParkServ { get; set; }
        public ICountryService iCountryServ { get; set; }
        public IProvinceService iProvinceServ { get; set; }
        public ICityService iCityServ { get; set; }

        private IConfiguration Config { get; }

        public ITCFService(IConfiguration Configuration)
        {
            IITCFUnitOfWork uow = new ITCFDbContext(Configuration.GetConnectionString("SystemBase"));
            IUserLegalFormServ = new UserLegalFormService(uow);
            IBusinessServ = new BusinessService(uow);
            IBusinessCategoryServ = new BusinessCategoryService(uow);
            IIndustrialParkServ = new IndustrialParkService(uow);
            iCountryServ = new CountryService(uow);
            iProvinceServ = new ProvinceService(uow);
            iCityServ = new CityService(uow);
        }


    }
}
