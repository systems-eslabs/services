using System;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace StorageService
{
    public static class Config
    {
        static Dictionary<string, string> _config = null;
        static string _projectEnvironment;

        static Config()
        {
            string JsonString = System.IO.File.ReadAllText("./config/config.json");
            _config = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonString);

            setConfigurations();
        }

        public static string projectEnvironment
        {
            get
            {
                return _projectEnvironment;
            }
        }

        static void setConfigurations()
        {
            _projectEnvironment = _config["projectEnvironment"];
        }


    }


}