using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RSA_Angular_.NET_CORE.RSA
{
    public class RsaHelper : IRsaHelper
    {
        private readonly RSACryptoServiceProvider _privateKey;

        public RsaHelper()
        {
            string private_pem = @"C:\Users\T_Unnati\source\repos\student_crud\student_crud\Keys\posvendor.key.pem";

            _privateKey = GetPrivateKeyFromPemFile(private_pem);


        }

        public string Decrypt(string encrypted)
        {
            var decryptedBytes = _privateKey.Decrypt(Convert.FromBase64String(encrypted), false);
            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
        }

        private RSACryptoServiceProvider GetPrivateKeyFromPemFile(string filePath)
        {
            using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(filePath)))
            {
                /* AsymmetricCipherKeyPair readKeyPair = (AsymmetricCipherKeyPair)new PemReader(privateKeyTextReader).ReadObject();
 
                 RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)readKeyPair.Private);
                 RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                 csp.ImportParameters(rsaParams);
                 return csp;*/


                PemReader pemReader = new PemReader(privateKeyTextReader); object obj = pemReader.ReadObject();
                if (obj is AsymmetricCipherKeyPair keyPair) { RsaPrivateCrtKeyParameters rsaPrivateParams = (RsaPrivateCrtKeyParameters)keyPair.Private; RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(rsaPrivateParams); RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(); rsaCsp.ImportParameters(rsaParams); return rsaCsp; }
                else if (obj is RsaPrivateCrtKeyParameters rsaPrivateParams) { RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(rsaPrivateParams); RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(); rsaCsp.ImportParameters(rsaParams); return rsaCsp; }
                else { throw new InvalidOperationException("Unsupported key format."); }
            }
        }

    }
}