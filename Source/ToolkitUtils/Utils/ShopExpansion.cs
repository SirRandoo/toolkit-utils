using System.IO;
using System.Xml.Serialization;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class ShopExpansion
    {
        public static T LoadData<T>(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                throw new IOException($"Directory {directory} does not exist!");
            }

            var serializer = new XmlSerializer(typeof(T));

            using StreamReader reader = File.OpenText(filePath);
            return (T) serializer.Deserialize(reader);
        }
    }
}
