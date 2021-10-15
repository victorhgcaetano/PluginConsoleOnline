using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleIntegracao
{
    public class ConnectionFactory
    {
        public static IOrganizationService GetCrmService()
        {
            string connectionString =
                "AuthType=OAuth;" +
                "Username=admin@t3grupo07.onmicrosoft.com;" +
                "Password=P@ssw0rd;" +
                "Url=https://org42afc0c4.crm2.dynamics.com;" +
                "AppId=2833bbff-9505-4b5d-8a00-c8a5a4feb401;" +
                "RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;";

            CrmServiceClient crmServiceClient = new CrmServiceClient(connectionString);
            return crmServiceClient.OrganizationWebProxyClient;
        }
    }
}
