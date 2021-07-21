using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Utilities.Encoders;

namespace VerificationTests
{
    public class Program
    {
        private static Base45Service Base45Service = new Base45Service();

        private static ZLibService ZLibService = new ZLibService();

        private static EnglandDataRepository EnglandDataRepository = new EnglandDataRepository();

        private static EUExampleRepository EUDataRepository = new EUExampleRepository();

        static void Main(string[] args)
        {
            var gbEngExamples = EnglandDataRepository.GetTestExamples().ToList();

            for(var i = 0; i < gbEngExamples.Count; i++)
            {
                try
                {
                    var example = gbEngExamples[i];

                    var coseObject = DecodeQR(example.Data);

                    var pk = Convert.FromBase64String(example.Key);

                    coseObject.VerifySignature(pk);

                    Console.WriteLine($"GB Example {i + 1}: Successfully Verified \r");
                }
                catch(Exception e)
                {
                    Console.WriteLine($"[ERROR]: GB Example {i + 1}: {e.Message} \r");
                }
            }

            var euExamples = EUDataRepository.GetTestExamples();

            foreach(var key in euExamples.Keys)
            {
                var examples = euExamples[key].ToList();

                for(var i = 0; i < examples.Count; i++)
                {
                    try
                    {
                        var example = examples[i];

                        var coseObject = DecodeQR(example.Data);

                        var cert = new X509Certificate2(Encoding.UTF8.GetBytes(example.Key));

                        var pk = cert.GetECDsaPublicKey();

                        coseObject.VerifySignature(pk.ExportSubjectPublicKeyInfo());

                        Console.WriteLine($"{key} Example {i + 1}: Successfully Verified \r");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"[ERROR]: {key} Example {i + 1}: {e.Message} \r");
                    }
                }
            }

            try
            {
                var ashExample = "HC1:6BFOXNXTSMAHN-HLLQ6E8 KLI HKHRSG24/4X6BMF6.UCOMIN6R*F7IA7T84.E7NOBTG90OARH9P1J4HGZJK4HGX2MGY8Z.C/NLE:FJED.IAHLCV5GVWNZIKXGGOA6FQREC5L64HX6IAS3DS2980IQODPUHLG$GAHLW 70SO:GOLIROGO3T59YLLYP-HQLTQ9R0+L6H6QD/5UNU7C174VS-EJWE0OEY81KFE-H1BT14T9AKPCPP7ZMDX02:C5QD8DJI7JSTNB959/5Y*ODKQR95Y16 96P/5CA7T5MNS5ZWJ6UCYWVDINV7J.$2BSH0CJA+O*CRGJCDAF4J4KWH0/C$WGL$SOP5YS8Q*VMRN %B2PECC46GD*JB$FVRK0DKETF542K1/4GZP+.TI7L8YD9*3JJP/UNL M KR$307E1T2";

                var ashCoseObject = DecodeQR(ashExample);

                var x = Convert.FromBase64String("DK7mHdv6qonWOThi86jJzW1XZef01CjPO1oon6WElkc=");
                var y = Convert.FromBase64String("Kj9/1P8ovejCRspTN0QBoxoe3tlPkQlKxbXP1lCGlX0=");

                var ecdsaKey = ECDsa.Create(new ECParameters
                {
                    Q = new ECPoint
                    {
                        X = x,
                        Y = y
                    }
                });

                ashCoseObject.VerifySignature(ecdsaKey.ExportSubjectPublicKeyInfo());

                Console.WriteLine("Ash Example: Successfully Verified! \r");
            }
            catch(Exception e)
            {
                Console.WriteLine($"[ERROR]: Ash Example: {e.Message} \r");
            }
        }

        static CoseSign1Object DecodeQR(string data)
        {
            var removedPrefix = data.Substring(4);
            var compressedBytes = Base45Service.Base45Decode(removedPrefix);
            var decompressedSignedCose = ZLibService.DecompressData(compressedBytes);
            var cborObject = CoseSign1Object.Decode(decompressedSignedCose);

            return cborObject;
        }
    }
}
