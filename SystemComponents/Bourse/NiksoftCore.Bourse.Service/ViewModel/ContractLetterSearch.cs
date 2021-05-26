using NiksoftCore.ViewModel;

namespace NiksoftCore.Bourse.Service
{
    public class ContractLetterSearch : BaseRequest
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public int ContractType { get; set; }
    }
}
