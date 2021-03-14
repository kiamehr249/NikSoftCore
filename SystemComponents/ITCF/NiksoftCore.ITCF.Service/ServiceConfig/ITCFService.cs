using Microsoft.Extensions.Configuration;

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
        IIntroductionService iIntroductionServ { get; set; }
        IProductService iProductServ { get; set; }
        IUserPurchaseService iUserPurchaseServ { get; set; }
        IUserModelService iUserModelServ { get; set; }
        IUserProfileService iUserProfileServ { get; set; }
        IProductFileService iProductFileServ { get; set; }
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
        public IIntroductionService iIntroductionServ { get; set; }
        public IProductService iProductServ { get; set; }
        public IUserPurchaseService iUserPurchaseServ { get; set; }
        public IUserModelService iUserModelServ { get; set; }
        public IUserProfileService iUserProfileServ { get; set; }
        public IProductFileService iProductFileServ { get; set; }


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
            iIntroductionServ = new IntroductionService(uow);
            iProductServ = new ProductService(uow);
            iUserPurchaseServ = new UserPurchaseService(uow);
            iUserModelServ = new UserModelService(uow);
            iUserProfileServ = new UserProfileService(uow);
            iProductFileServ = new ProductFileService(uow);
        }


    }
}
