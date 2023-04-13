using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Shared.Constants
{
    public static class SystemConstants
    {
        public const string MainConnectionString = "BaseSourceDbConnection";
        public const string DefaultLanguageId = "DefaultLanguageId";
        public const string Token = "Token";
        public const string BackendApiClient = "BackendApiClient";
        public const string Payment9PayApiClient = "Payment9PayApiClient";
        public const string LanguageCulture = ".AspNetCore.Culture";
        public const string CurrencyCulture = "Currency";
        public const short ErrorList = -1;
        public const short ErrorMess = -2;
        public const short Timeout = -3;
        public const short Success = 1;
        public const short NotFound = 0;
        public const string URLImageBlogCategory = "uploads/blogcategory/";
        public const string URLImageUser = "uploads/user/";
        public const string LanguageVietnam = "vi";
        public const string LanguageEnglish = "en";
        public const string LanguageKorean = "ko";
        public const string LanguageThailan = "th";
        public const string LanguageChina = "zh";
        public const string LanguageIndonesia = "id";
        public const string LanguageDefault = "vi";
        public const string MT4APICLIENT = "MT4ApiClient";
        public const string MT5APICLIENT = "MT4ApiClient";
        public const string BinanceApiClient = "BinanceApiClient";
    }
    public static class ErrorCodeJson
    {
        public const short ErrorList = -1;
        public const short ErrorMess = -2;
        public const short Timeout = -3;
        public const short Success = 1;
    }
}
