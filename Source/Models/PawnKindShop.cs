using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [XmlRoot("PawnKindShop")]
    public class PawnKindShop
    {
        [XmlIgnore] public string FilePath;

        public List<PawnKindProduct> Products;
        [XmlIgnore] public XmlSerializer Serializer;

        public static PawnKindShop Load(string filePath)
        {
            var serializer = new XmlSerializer(typeof(PawnKindShop));

            using StreamReader reader = File.OpenText(filePath);
            var result = (PawnKindShop) serializer.Deserialize(reader);
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
