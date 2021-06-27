namespace NiksoftCore.FormBuilder.Service
{
    public interface IFormBuilderService
    {
        FormDbContext dbContext { get; }
        IBourseUserService iBourseUserServ { get; set; }
        IUserProfileService iUserProfileServ { get; set; }
        IFormService iFormServ { get; set; }
        IFormControlService iFormControlServ { get; set; }
        IControlItemService iControlItemServ { get; set; }
        IFormDataService iFormDataServ { get; set; }
    }

    public class FormBuilderService : IFormBuilderService
    {
        public FormDbContext dbContext { get; }
        public IBourseUserService iBourseUserServ { get; set; }
        public IUserProfileService iUserProfileServ { get; set; }
        public IFormService iFormServ { get; set; }
        public IFormControlService iFormControlServ { get; set; }
        public IControlItemService iControlItemServ { get; set; }
        public IFormDataService iFormDataServ { get; set; }

        public FormBuilderService(string connection)
        {
            dbContext = new FormDbContext(connection);
            IFormUnitOfWork uow = dbContext;
            iBourseUserServ = new BourseUserService(uow);
            iUserProfileServ = new UserProfileService(uow);
            iFormServ = new FormService(uow);
            iFormControlServ = new FormControlService(uow);
            iControlItemServ = new ControlItemService(uow);
            iFormDataServ = new FormDataService(uow);
        }

    }
}
