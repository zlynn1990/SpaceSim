using System.IO;
using System.Xml.Serialization;

namespace OpenCLWrapper
{
    public static class Serializer
    {
        public static T Read<T>(string path)
        {
            var deserializer = new XmlSerializer(typeof(T));

            object output;

            using (var reader = new StreamReader(path))
            {
                output = deserializer.Deserialize(reader);

                reader.Close();   
            }

            return (T)output;
        }

        public static void Write<T>(T contract, string path)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, contract);
                writer.Close();
            }
        }
    }
}
