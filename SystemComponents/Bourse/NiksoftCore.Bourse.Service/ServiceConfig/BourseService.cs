using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;

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
        ISettingService iSettingServ { get; set; }
        IContractLetterService iContractLetterServ { get; set; }
        IBaseTransactionService iBaseTransactionServ { get; set; }
        IPaymentReceiptService iPaymentReceiptServ { get; set; }
        IBranchAreaService iBranchAreaServ { get; set; }
        IBranchAdLeaderService iBranchAdLeaderServ { get; set; }
        IBranchAdvertiserService iBranchAdvertiserServ { get; set; }

        List<ConsultantReport> GetConsultantReport(string Code, int Start, int End);

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
        public ISettingService iSettingServ { get; set; }
        public IContractLetterService iContractLetterServ { get; set; }
        public IBaseTransactionService iBaseTransactionServ { get; set; }
        public IPaymentReceiptService iPaymentReceiptServ { get; set; }
        public IBranchAreaService iBranchAreaServ { get; set; }
        public IBranchAdLeaderService iBranchAdLeaderServ { get; set; }
        public IBranchAdvertiserService iBranchAdvertiserServ { get; set; }

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
            iSettingServ = new SettingService(uow);
            iContractLetterServ = new ContractLetterService(uow);
            iBaseTransactionServ = new BaseTransactionService(uow);
            iPaymentReceiptServ = new PaymentReceiptService(uow);
            iBranchAreaServ = new BranchAreaService(uow);
            iBranchAdLeaderServ = new BranchAdLeaderService(uow);
            iBranchAdvertiserServ = new BranchAdvertiserService(uow);
        }

        public List<ConsultantReport> GetConsultantReport(string Code, int Start, int End)
        {
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                var query = String.Format("EXEC Sp_GetConsultantReport @MarketerCode = {0}, @StartPeriod = {1}, @EndPereiod = {2};", Code, Start, End);
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                dbContext.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<ConsultantReport>();

                    while (result.Read())
                    {
                        entities.Add(new ConsultantReport
                        {
                            Period = result.IsDBNull(0) ? 0 : result.GetFieldValue<int>(0),
                            MarketerCode = result.IsDBNull(1) ? "" : result.GetFieldValue<string>(1),
                            MarketerName = result.IsDBNull(2) ? "" : result.GetFieldValue<string>(2),
                            ConsultantCode = result.IsDBNull(3) ? "" : result.GetFieldValue<string>(3),
                            ConsultantName = result.IsDBNull(4) ? "" : result.GetFieldValue<string>(4),
                            CustomerCount = result.IsDBNull(5) ? 0 : result.GetFieldValue<int>(5),
                            TotalInputAmount = result.IsDBNull(6) ? 0 : result.GetFieldValue<double>(6),
                            TotalConsultantWage = result.IsDBNull(7) ? 0 : result.GetFieldValue<double>(7),
                        });
                    }

                    return entities;
                }
            }

        }

    }
}
