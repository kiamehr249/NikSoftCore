using NiksoftCore.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.Utilities
{
    public static class NikTools
    {
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static async Task<SaveFileResponse> SaveFileAsync(SaveFileRequest request)
        {
            var result = new SaveFileResponse();
            if (request.File == null)
            {
                result.Success = false;
                result.Message = "File request is empty";
                return result;
            }

            if (string.IsNullOrWhiteSpace(request.RootPath))
            {
                result.Success = false;
                result.Message = "Root Path is empty";
                return result;
            }

            if (request.File.Length > 0)
            {
                var fileName = Path.GetFileName(request.File.FileName);
                var newName = RandomString(6) + "_" + fileName.Replace(" ", "");
                var folderPath = request.RootPath + "/wwwroot/" + request.UnitPath + "/" + newName;

                using (var stream = System.IO.File.Create(folderPath))
                {
                    await request.File.CopyToAsync(stream);
                    result.FilePath = request.UnitPath + "/" + newName;
                    result.FullPath = folderPath;
                }
            }

            result.Success = true;
            result.Message = "Save Successed";

            return result;
        }

        public static void RemoveFile(RemoveFileRequest request)
        {
            if (File.Exists(request.RootPath + "/wwwroot/" + request.FilePath))
            {
                File.Delete(request.RootPath + "/wwwroot/" + request.FilePath);
            }
            
        }

        public static string PersianToEnglish(this string persianStr)
        {
            Dictionary<char, char> LettersDictionary = new Dictionary<char, char>
            {
                ['۰'] = '0',
                ['۱'] = '1',
                ['۲'] = '2',
                ['۳'] = '3',
                ['۴'] = '4',
                ['۵'] = '5',
                ['۶'] = '6',
                ['۷'] = '7',
                ['۸'] = '8',
                ['۹'] = '9'
            };
            foreach (var item in persianStr)
            {
                if (LettersDictionary.ContainsKey(item))
                {
                    persianStr = persianStr.Replace(item, LettersDictionary[item]);
                }
            }
            return persianStr;
        }

        public static string GetExtention(this string fileName)
        {
            var arrList = fileName.Split('.');
            if (arrList.Length < 2)
            {
                return "";
            }
            return arrList[arrList.Length - 1];
        }

        public static string GetDisplayName(this System.Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }

        public static string RandomString(int size, string key = "", bool lowerCase = false)
        {
            Random _random = new Random();
            var builder = new StringBuilder(size); 
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  
            
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            builder.Append(key);
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }
}
