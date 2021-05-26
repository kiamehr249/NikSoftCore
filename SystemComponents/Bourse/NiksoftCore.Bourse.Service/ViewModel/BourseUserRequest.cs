using Microsoft.AspNetCore.Http;
using NiksoftCore.ViewModel;
using System;

namespace NiksoftCore.Bourse.Service
{
    public class BourseUserRequest
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }

        public int ProfileId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string UserCode { get; set; }
        public string AuthId { get; set; }
        public string CompanyName { get; set; }
        public string NCode { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string BirthDate { get; set; }
        public IFormFile AvatarFile { get; set; }
        public string Avatar { get; set; }
        public IFormFile IdCardFile { get; set; }
        public string IdCardImage { get; set; }
        public IFormFile NCardFile { get; set; }
        public string NCardImage { get; set; }
        public ProfileStatus Status { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public int Gender { get; set; }
        public int ProfileType { get; set; }

        public int BankId { get; set; }
        public string PAN { get; set; }
        public string IBAN { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }

        public int BranchId { get; set; }
    }
}
