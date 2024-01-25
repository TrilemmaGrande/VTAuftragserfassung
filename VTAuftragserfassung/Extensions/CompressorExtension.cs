using System.IO.Compression;
using System.Text.Json;

namespace VTAuftragserfassung.Extensions
{
    public static class CompressorExtension
    {
        public static byte[] SerializeAndCompress(this object obj)
        {
            using MemoryStream ms = new MemoryStream();
            using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress, true))
            {
                JsonSerializer.Serialize(zs, obj);
            }
            return ms.ToArray();
        }

        public static T? DecompressAndDeserialize<T>(this byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (GZipStream zs = new GZipStream(ms, CompressionMode.Decompress, true))
            {
                return JsonSerializer.Deserialize<T?>(zs);
            }
        }
    }
}