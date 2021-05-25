namespace NiksoftCore.Bourse.Service
{
    public interface IBourseService
    {
        BourseDbContext dbContext { get; }
        IBourseUserService iBourseUserServ { get; set; }
        IUserProfileService iUserProfileServ { get; set; }
        IUserBankAccountService iUserBankAccountServ { get; set; }
        IFeeService iFeeServ { get; set; }
        IBranchService iBranchServ { get; set; }
        IBranchMasterService iBranchMasterServ { get; set; }
        IBranchMarketerService iBranchMarketerServ { get; set; }
        IBranchConsultantService iBranchConsultantServ { get; set; }
        IBranchUserService iBranchUserServ { get; set; }
        IContractService iContractServ { get; set; }
        IMediaCategoryService iMediaCategoryServ { get; set; }
        IMediaService iMediaServ { get; set; }
        IContractFeeService iContractFeeServ { get; set; }
    }

    public class BourseService : IBourseService
    {
        public BourseDbContext dbContext { get; }
        public IBourseUserService iBourseUserServ { get; set; }
        public IUserProfileService iUserProfileServ { get; set; }
        public IUserBankAccountService iUserBankAccountServ { get; set; }
        public IFeeService iFeeServ { get; set; }
        public IBranchService iBranchServ { get; set; }
        public IBranchMasterService iBranchMasterServ { get; set; }
        public IBranchMarketerService iBranchMarketerServ { get; set; }
        public IBranchConsultantService iBranchConsultantServ { get; set; }
        public IBranchUserService iBranchUserServ { get; set; }
        public IContractService iContractServ { get; set; }
        public IMediaCategoryService iMediaCategoryServ { get; set; }
        public IMediaService iMediaServ { get; set; }
        public IContractFeeService iContractFeeServ { get; set; }

        public BourseService(string connection)
        {
            dbContext = new BourseDbContext(connection);
            IBourseUnitOfWork uow = dbContext;
            iBourseUserServ = new BourseUserService(uow);
            iUserProfileServ = new UserProfileService(uow);
            iUserBankAccountServ = new UserBankAccountService(uow);
            iFeeServ = new FeeService(uow);
            iBranchServ = new BranchService(uow);
            iBranchMasterServ = new BranchMasterService(uow);
            iBranchMarketerServ = new BranchMarketerService(uow);
            iBranchConsultantServ = new BranchConsultantService(uow);
            iBranchUserServ = new BranchUserService(uow);
            iContractServ = new ContractService(uow);
            iMediaCategoryServ = new MediaCategoryService(uow);
            iMediaServ = new MediaService(uow);
            iContractFeeServ = new ContractFeeService(uow);
        }

    }
}
