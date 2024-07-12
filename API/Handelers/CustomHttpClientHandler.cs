using System.Security.Cryptography.X509Certificates;

public class CustomHttpClientHandler : HttpClientHandler
{
    public CustomHttpClientHandler()
    {
        ClientCertificateOptions = ClientCertificateOption.Manual;
        SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        // Certificado embutido
        var clientCertString = "MIICXgIBAAKBgQ ..."; // Insira aqui a string do certificado kl_scanengine_private.p12 codificado em Base64
        var certBytes = Convert.FromBase64String(clientCertString);
        var clientCertificate = new X509Certificate2(certBytes, "Edoc!2"); // Certificado e senha

        ClientCertificates.Add(clientCertificate);

        // Ignorar a validação do certificado do servidor
        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) =>
        {
            // Aqui você pode adicionar lógica para validar o certificado do servidor.
            return true;
        };
    }
}
