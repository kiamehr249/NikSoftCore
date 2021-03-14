namespace NiksoftCore.SystemBase.Service
{
    public interface ISystemBaseService
    {
        ISystemSettingService iSystemSettingServ { get; set; }
        IPortalLanguageService iPortalLanguageServ { get; set; }
        IPanelMenuService iPanelMenuService { get; set; }
        IUserProfileService iUserProfileServ { get; set; }
        ICountryService iCountryServ { get; set; }
        IProvinceService iProvinceServ { get; set; }
        ICityService iCityServ { get; set; }
        ISystemFileService iSystemFileServ { get; set; }
        IContentCategoryService iContentCategoryServ { get; set; }
        IGeneralContentService iGeneralContentServ { get; set; }
        IContentFileService iContentFileServ { get; set; }
        IMenuCategoryService iMenuCategoryServ { get; set; }
        IMenuService iMenuServ { get; set; }
        INikUserService iNikUserServ { get; set; }
        INikRoleService iNikRoleServ { get; set; }
        IBaseImportService iBaseImportServ { get; set; }
    }

    public class SystemBaseService : ISystemBaseService
    {
        public ISystemSettingService iSystemSettingServ { get; set; }
        public IPortalLanguageService iPortalLanguageServ { get; set; }
        public IPanelMenuService iPanelMenuService { get; set; }
        public IUserProfileService iUserProfileServ { get; set; }
        public ICountryService iCountryServ { get; set; }
        public IProvinceService iProvinceServ { get; set; }
        public ICityService iCityServ { get; set; }
        public ISystemFileService iSystemFileServ { get; set; }
        public IContentCategoryService iContentCategoryServ { get; set; }
        public IGeneralContentService iGeneralContentServ { get; set; }
        public IContentFileService iContentFileServ { get; set; }
        public IMenuCategoryService iMenuCategoryServ { get; set; }
        public IMenuService iMenuServ { get; set; }
        public INikUserService iNikUserServ { get; set; }
        public INikRoleService iNikRoleServ { get; set; }
        public IBaseImportService iBaseImportServ { get; set; }

        public SystemBaseService(string connection)
        {
            ISystemUnitOfWork uow = new SystemBaseDbContext(connection);
            iSystemSettingServ = new SystemSettingService(uow);
            iPortalLanguageServ = new PortalLanguageService(uow);
            iPanelMenuService = new PanelMenuService(uow);
            iUserProfileServ = new UserProfileService(uow);
            iCountryServ = new CountryService(uow);
            iProvinceServ = new ProvinceService(uow);
            iCityServ = new CityService(uow);
            iSystemFileServ = new SystemFileService(uow);
            iContentCategoryServ = new ContentCategoryService(uow);
            iGeneralContentServ = new GeneralContentService(uow);
            iContentFileServ = new ContentFileService(uow);
            iMenuCategoryServ = new MenuCategoryService(uow);
            iMenuServ = new MenuService(uow);
            iNikUserServ = new NikUserService(uow);
            iNikRoleServ = new NikRoleService(uow);
            iBaseImportServ = new BaseImportService(uow);
        }


    }
}
