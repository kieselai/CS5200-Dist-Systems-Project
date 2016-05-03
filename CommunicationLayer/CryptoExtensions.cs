using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLayer
{
    public static class CryptoExtensions {
        public static PublicKey ToPublicKey(this RSAParameters rsaParams) {
            return new PublicKey {
                Exponent = rsaParams.Exponent,
                Modulus  = rsaParams.Modulus
            };
        }

        public static RSAPKCS1SignatureFormatter UseSHA1(this RSAPKCS1SignatureFormatter rsaSig) {
            rsaSig.SetHashAlgorithm("SHA1");
            return rsaSig;
        }

        public static RSAPKCS1SignatureDeformatter UseSHA1(this RSAPKCS1SignatureDeformatter rsaDeformatter) {
            rsaDeformatter.SetHashAlgorithm("SHA1");
            return rsaDeformatter;
        }
    }
}
