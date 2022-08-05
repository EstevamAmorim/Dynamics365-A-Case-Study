using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class ConnectionFactory
    {
        public static IOrganizationService GetService()
        {
            string ConnectionString =
                "AuthType = OAuth; " +
                "Username = EstevamAmorim@Dynacoop400.onmicrosoft.com; " +
                "Password = Blastoise098; " +
                "Url = https://org2e8c0741.crm2.dynamics.com;" +
                "AppId=104d742a-092e-41d3-91aa-fe335e091291;" +
                "RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;";

            CrmServiceClient crmServiceClient = new CrmServiceClient(ConnectionString);

            return crmServiceClient.OrganizationWebProxyClient;

        }

    }
}
