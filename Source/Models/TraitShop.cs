using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [XmlRoot("TraitShop")]
    public class TraitShop
    {
        [XmlIgnore] public string FilePath;

        public List<TraitProduct> Products;
        [XmlIgnore] public XmlSerializer Serializer;

        public static TraitShop Load(string filePath)
        {
            var serializer = new XmlSerializer(typeof(TraitShop));

            using StreamReader reader = File.OpenText(filePath);
            var result = (TraitShop) serializer.Deserialize(reader);
            result.FilePath = filePath;
            result.Serializer = serializer;

            return result;
        }

        public void Save()
        {
            if (FilePath.NullOrEmpty())
            {
                return;
            }

            using FileStream writer = File.Open(FilePath, FileMode.Create, FileAccess.Write);
            Serializer.Serialize(writer, this);
        }

        public void Save(string filePath)
        {
            if (filePath.NullOrEmpty())
            {
                return;
            }

            using FileStream writer = File.Open(filePath, FileMode.Create, FileAccess.Write);
            Serializer.Serialize(writer, this);
        }
    }
}
