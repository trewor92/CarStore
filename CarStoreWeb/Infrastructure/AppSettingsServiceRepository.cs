using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Infrastructure
{
    public class AppSettingsServiceRepository
    {
        private IConfiguration _configuration;
        public AppSettingsServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual string GetUserName()
        {
            string userName = _configuration["Data:WebApiSettings:UserName"];
            if (userName == null)
                throw new Exception("NULL_USERNAME_EXCEPTION");
            return userName;
        }

        public virtual string GetHashPassword()
        {
            string hashPassword = _configuration["Data:WebApiSettings:CryptPassword"];
            if (hashPassword == null)
                throw new Exception("NULL_HASHPASSWORD_EXCEPTION");
            return hashPassword;
        }

        public virtual string GetAppIdentityConnString()
        {
            string appIdentityConnString = _configuration["Data:AppIdentityDbContext:ConnectionString"];
            if (appIdentityConnString == null)
                throw new Exception("NULL_APPIDENTITYCONNSTRING_EXCEPTION");
            return appIdentityConnString;
        }

        public virtual string GetWebApiHostUrl()
        {
            string webApiHostUrl = _configuration["Data:WebApiHostUrl"];
            if (webApiHostUrl == null)
                throw new Exception("NULL_WEBAPIHOSTURL_EXCEPTION");
            return webApiHostUrl;
        }

        private string GetLoginPath()
        {
            string loginPath = _configuration["Data:WebApiSettings:LoginPath"];
            if (loginPath == null)
                throw new Exception("NULL_LOGINPATH_EXCEPTION");
            return loginPath;
        }

        private string GetRefreshPath()
        {
            string refreshPath = _configuration["Data:WebApiSettings:RefreshPath"];
            if (refreshPath == null)
                throw new Exception("NULL_REFRESHPATH_EXCEPTION");
            return refreshPath;
        }

        private string GetCarPath()
        {
            string carPath = _configuration["Data:WebApiSettings:CarPath"];
            if (carPath == null)
                throw new Exception("NULL_CARPATH_EXCEPTION");
            return carPath;
        }

        public virtual int GetPageSize()
        {
            string pageSize = _configuration["Data:AppSettings:NoticeController:IntPageSize"];
            if (pageSize == null)
                throw new Exception("NULL_CARPATH_EXCEPTION");
            return Convert.ToInt32(pageSize);
        }
        

        public virtual string GetRefreshUrl()
        {
            return GetWebApiHostUrl() + GetRefreshPath();
        }

        public virtual string GetLoginUrl()
        {
            return GetWebApiHostUrl() + GetLoginPath();
        }

        public virtual string GetCarUrl()
        {
            return GetWebApiHostUrl() + GetCarPath();
        }

    }
}
