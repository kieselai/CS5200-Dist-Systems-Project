using System.Security.Cryptography;
using SharedObjects;

namespace CommunicationLayer
{
    public class CryptoService {
        protected static RSACryptoServiceProvider RSA { get; set; }
        protected static RSAParameters PublicKeyInfo  { get; set; }
        public static PublicKey PublicKey { get { return PublicKeyInfo.ToPublicKey(); } }

        static CryptoService() {
            RSA = new RSACryptoServiceProvider();
            PublicKeyInfo = RSA.ExportParameters(true);
        }

        static public byte[] HashAndSign(byte[] data) {
            return SignBytes( HashBytes(data) );
        }

        static byte[] HashBytes(byte[] data) {
            return new SHA1Managed().ComputeHash(data);
        }

        static byte[] SignBytes(byte[] bytes) {
            return new RSAPKCS1SignatureFormatter(RSA).UseSHA1().CreateSignature(bytes);
        }

        static public bool VerifySignature(byte[] data, byte[] givenSignature) {
            return CompareEquals( HashBytes(data), givenSignature);
        }

        static bool CompareEquals(byte[] hash, byte[] givenSignature) {
            return new RSAPKCS1SignatureDeformatter(RSA).UseSHA1().VerifySignature(hash, givenSignature);
        }
    }
}
