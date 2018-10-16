using System;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace mailLibrary
{
    public static class Config
    {
        static Dictionary<string, string> _config = null;
        static string _mailClientId;
        static string _mailClientSecret;
        static string _mailAccessToken;
        static string _mailRefreshToken;
        static string _serviceMailId;

        static Config()
        {
            string JsonString = System.IO.File.ReadAllText("./config/config.json");
            _config = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonString);

            setConfigurations();
        }


        public static string mailClientId
        {
            get
            {
                return _mailClientId;
            }
        }
        public static string mailAccessToken
        {
            get
            {
                return _mailAccessToken;
            }
        }
        public static string mailRefreshToken
        {
            get
            {
                return _mailRefreshToken;
            }
        }
        public static string serviceMailId
        {
            get
            {
                return _serviceMailId;
            }
        }
        public static string mailClientSecret
        {
            get
            {
                return _mailClientSecret;
            }
        }

        static void setConfigurations()
        {
            _mailClientId = _config["mailClientId"];
            _mailClientSecret = _config["mailClientSecret"];
            _mailAccessToken = _config["mailAccessToken"];
            _mailRefreshToken = _config["mailRefreshToken"];
            _serviceMailId = _config["serviceMailId"];
        }


    }


}