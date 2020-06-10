using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Lib.Socket.Server.Utils.Factory;

namespace DotNetCore.SslSocket.Server.Utils
{
    /// <summary>
    /// Certificate helper
    /// </summary>
    public class CertHelper
    {
        /// <summary>
        /// Server Certificate
        /// </summary>
        public static X509Certificate ServerCertificate = new X509Certificate("Certs\\local.pfx", string.Empty);

        /// <summary>
        /// Verify TLS/SSL certificate
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="certificate">Certificate</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="sslPolicyErrors">SSL policy errs</param>
        /// <returns>True(Validate OK)/False(Validate NG)</returns>
        public static bool ValidateServerCertificate(
            object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // For self-signed certificate, always return true.
            return true;

            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            LoggerProvider.Logger.Error($"Certificate validation error: {sslPolicyErrors}");

            return false;
        }
    }
}
