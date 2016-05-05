using System.Security.Cryptography;
using SharedObjects;

namespace CommunicationLayer
{
    public class CryptoService {
        protected static RSACryptoServiceProvider OwnRSAProvider { get; set; }
        protected static RSAParameters PublicKeyInfo  { get; set; }
        public static PublicKey PublicKey { get { return PublicKeyInfo.ToPublicKey(); } }

        static CryptoService() {
            OwnRSAProvider = new RSACryptoServiceProvider();
            PublicKeyInfo = OwnRSAProvider.ExportParameters(true);
        }

        static public byte[] HashAndSign(byte[] data) {
            return SignBytes( HashBytes(data) );
        }

        static byte[] HashBytes(byte[] data) {
            return new SHA1Managed().ComputeHash(data);
        }

        static byte[] SignBytes(byte[] bytes) {
            return new RSAPKCS1SignatureFormatter(OwnRSAProvider).UseSHA1().CreateSignature(bytes);
        }

        static public bool VerifySignature(PublicKey senderPublicKey, byte[] data, byte[] givenSignature) {
            var rsaSender = new RSACryptoServiceProvider().ImportKey(senderPublicKey);
            return CompareEquals(rsaSender, HashBytes(data), givenSignature);
        }

        static bool CompareEquals(RSACryptoServiceProvider rsa, byte[] hash, byte[] givenSignature) {
            return new RSAPKCS1SignatureDeformatter(rsa).UseSHA1().VerifySignature(hash, givenSignature);
        }
    }
}
