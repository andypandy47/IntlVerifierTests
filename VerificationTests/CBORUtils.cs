using PeterO.Cbor;

namespace VerificationTests
{
    public class CBORUtils
    {
        public static string ToJson(byte[] cborDataFormatBytes)
        {
            var cborObjectFromBytes = CBORObject.DecodeFromBytes(cborDataFormatBytes);
            var jsonString = cborObjectFromBytes.ToJSONString();

            return jsonString;
        }
    }
}
