using Microsoft.EntityFrameworkCore;
using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace NiksoftCore.SystemBase.Service
{
    public interface ISystemBaseService
    {
        SystemBaseDbContext dbContext { get; }
        ISystemSettingService iSystemSettingServ { get; set; }
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
        List<SpImportData> GetSubmitUsers(int start, int size);
    }

    public class SystemBaseService : ISystemBaseService
    {
        public SystemBaseDbContext dbContext { get; }
        public ISystemSettingService iSystemSettingServ { get; set; }
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

        public SystemBaseService(string connection)
        {
            dbContext = new SystemBaseDbContext(connection);
            ISystemUnitOfWork uow = dbContext;
            iSystemSettingServ = new SystemSettingService(uow);
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
        }

        public List<SpImportData> GetSubmitUsers(int start, int size)
        {
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                var query = String.Format("EXEC Sp_ImportData @Start = {0}, @Size = {1};", start, size);
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                dbContext.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<SpImportData>();

                    while (result.Read())
                    {
                        entities.Add(new SpImportData
                        {
                            UserId = result.IsDBNull(0) ? 0 : result.GetFieldValue<int>(0),
                            Email = result.IsDBNull(1) ? "" : result.GetFieldValue<string>(1),
                            Name = result.IsDBNull(2) ? "" : result.GetFieldValue<string>(2),
                            Address = result.IsDBNull(3) ? "" : result.GetFieldValue<string>(3),
                            Phone = result.IsDBNull(4) ? "" : result.GetFieldValue<string>(4)
                        });
                    }

                    return entities;
                }
            }

        }


    }
}
