using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eslabs_storage_api.Controllers
{
     
    public class StorageController : Controller
    {
        [HttpPost]
        [Route("api/Storage/FormDataUpload")]
        public string FormDataUpload()
        {
            var file = Request.Form.Files[0];

            eslabs_storage.Storage storage = new eslabs_storage.Storage();

            eslabs_storage.StorageResposne objStoreage = storage.FormDataUpload(file.FileName, "enbloc", file, true);

            return JsonConvert.SerializeObject(objStoreage);

        }

        [HttpPost]
        [Route("api/Storage/BinaryUpload")]
        public string BinaryUpload(string base64, string name)
        {

            byte[] data = Convert.FromBase64String(base64);
            string fileName = Guid.NewGuid() + "_" + name;

            eslabs_storage.Storage storage = new eslabs_storage.Storage();

            eslabs_storage.StorageResposne objStoreage = storage.BinaryUpload(fileName, "enbloc", data, true);

            return JsonConvert.SerializeObject(objStoreage);
        }



    }
}
