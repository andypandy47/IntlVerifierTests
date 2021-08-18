using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Utilities.Encoders;
using static System.Security.Cryptography.ECCurve;

namespace VerificationTests
{
    public class Program
    {
        private static Base45Service Base45Service = new Base45Service();

        private static ZLibService ZLibService = new ZLibService();

        private static EnglandDataRepository EnglandDataRepository = new EnglandDataRepository();

        private static EUExampleRepository EUDataRepository = new EUExampleRepository();

        private static BarcodeApiRepository BarcodeApiRepository = new BarcodeApiRepository();

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


            // 2D Barcode API
            var barcodeApiExamples =  BarcodeApiRepository.GetTestExamples().ToList();
            for (var i = 0; i < barcodeApiExamples.Count; i++)
            {
                try
                {
                    var example = barcodeApiExamples[i];

                    var coseObject = DecodeQR(example.Data);

                    var pk = Convert.FromBase64String(example.Key);

                    coseObject.VerifySignature(pk);

                    //var decoded = coseObject.GetJson();

                    Console.WriteLine($"2D Barcode API Example {i + 1}: Successfully Verified \r");
                    //Console.Write(decoded);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ERROR]: GB Example {i + 1}: {e.Message} \r");
                }
            }


            // OLD 2D BARCODE API WAY TO CHECK
            //try
            //{
            //    var ashExample = "HC1:6BFOXNXTSMAHN-HLLQQA8KN4 G2KR4G.4.HL44W/ZAOJA4%TA-APF6R:5SVBHABVCNN95ZTM7W5HHP4F5G+P%YQ+GOUSPA1R$HQUHP.SQIHPAHPGSOH767-RCTQHCRX39JW6UA7:MPJW6UX4U96L*KDYPWGO*88+EQHCRXQ6IWMAK8DK4LC6DQ4$84-/5AJOS:7A7NG16 YK:84J/MKQ5DD7M47:/6N9R%EPXCROGO COPAD7EDA.D90I/EL6KKYHIL4OTJLGY83DE0OA0D9E2LBHHGKLO-K%FGLIA5D8MJKQJK6HMMBIE2K5OI9YI:8D%F1$$9*EREA7IB65C94JB0J9JVL 3F6LFCMH%-BPKD4JB781E34JY3QCE/YU3Y9UM97H98$QP3R8BHGHRHALQ%PUX8XH4YH5$TVNAW65FO.C LT:G7JCQEV3B*CR PSFV96U:+JJUC+5RAWBKJIY$R:ATRMUXZ7LDR7HB6*EK3RQ%7XFWWOPM20/4RB0";

            //    var ashCoseObject = DecodeQR(ashExample);

            //    var x = Convert.FromBase64String("DK7mHdv6qonWOThi86jJzW1XZef01CjPO1oon6WElkc=");
            //    var y = Convert.FromBase64String("Kj9/1P8ovejCRspTN0QBoxoe3tlPkQlKxbXP1lCGlX0=");

            //    var ecdsaKey = ECDsa.Create(new ECParameters
            //    {
            //        Curve = NamedCurves.nistP256,
            //        Q = new ECPoint
            //        {
            //            X = x,
            //            Y = y
            //        }
            //    });

            //    ashCoseObject.VerifySignature(ecdsaKey.ExportSubjectPublicKeyInfo());



            //    var fish = ashCoseObject.GetJson();

            //    Console.WriteLine("Ash Example: Successfully Verified! \r");
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine($"[ERROR]: Ash Example: {e.Message} \r");
            //}
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
