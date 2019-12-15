using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreRest.Infrastructure
{
    public class AppSettingsServiceRepository
    {
        private IConfiguration _configuration;
        public AppSettingsServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual string GetApplicationConnString()
        {
            string applicationConnString = _configuration["Data:ApplicationDbContext:ConnectionString"];
            if (applicationConnString == null)
                throw new Exception("NULL_APPLICATIONCONNSTRING_EXCEPTION");
            return applicationConnString;
        }

        public virtual string GetAppIdentityConnString()
        {
            string appIdentityConnString = _configuration["Data:AppIdentityDbContext:ConnectionString"];
            if (appIdentityConnString == null)
                throw new Exception("NULL_APPIDENTITYCONNSTRING_EXCEPTION");
            return appIdentityConnString;
        }

        public virtual string GetLoginProviderName()
        {
            string loginProviderName = _configuration["Data:AppSettings:LoginProviderName"];
            if (loginProviderName == null)
                throw new Exception("NULL_LOGINPROVIDERNAME_EXCEPTION");
            return loginProviderName;
        }

        public virtual string GetJwtKey()
        {
            string jwtKey = _configuration["Data:AppSettings:JwtKey"];
            if (jwtKey == null)
                throw new Exception("NULL_JWTKEY_EXCEPTION");
            return jwtKey;
        }

        public virtual double GetJwtExpireSec()
        {
            string jwtExpireSec = _configuration["Data:AppSettings:JwtExpireSec"];
            if (jwtExpireSec == null)
                throw new Exception("NULL_JWTEXPIRESEC_EXCEPTION");
            return Convert.ToDouble(jwtExpireSec);
        }

    }
}
