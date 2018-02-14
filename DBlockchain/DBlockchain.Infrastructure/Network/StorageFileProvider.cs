using Newtonsoft.Json;
using System;
using System.IO;

namespace DBlockchain.Infrastructure.Network
{
    public static class StorageFileProvider<T>
    {
        public static T GetModel(string path)
        {
            T model;
            var content = File.ReadAllText(path);

            if (content == string.Empty || content == null)
            {
                return default(T);
            }
            else
            {
                model = JsonConvert.DeserializeObject<T>(content);
            }

            return model;
        }

        public static void SetModel(string path, T model)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(model));
        }
    }
}
