using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        static void error()
        {
            Console.WriteLine("Digite um valor valido!!!");
        }
        static string GetString(string s)
        {
            string str = "";
            while(str == "")
            {
                Console.WriteLine(s);
                str = Console.ReadLine();

                if (str == "")
                    error();
            } 
            return str;
        }
        
        static decimal GetFloat(string s)
        {
            decimal result;
 
            while(true)
            {
                Console.WriteLine(s);
                try
                {
                    result = Convert.ToDecimal(Console.ReadLine());
                    return result;
                }
                catch (Exception)
                {
                    error();
                }
            }
            
        }

        static int GetInt(string s)
        {
            int result;

            while (true)
            {
                Console.WriteLine(s);
                try
                {
                    result = Convert.ToInt32(Console.ReadLine());
                    return result;
                }
                catch (Exception)
                {
                    error();
                }
            }
        }

        static OptionSetValue GetAccountType(string s)
        {
            int accountType;

            do
            {
                accountType = GetInt(s);
            } while (accountType != 1  && accountType != 2);

            if (accountType == 1)
            {
                return new OptionSetValue(155780000);
            }
            else
            {
                return new OptionSetValue(155780001);
            }
        }

        static Entity GetRecommendedBy(IOrganizationService service)
        {
            Entity recommendedBy;
            
            while(true)
            {
                string firstname = GetString("Digite o primeiro nome de quem te indicou:");
                string lastname = GetString("Digite o ultimo nome de quem te indicou:");

                recommendedBy = GetContactIDByName(service, firstname, lastname);

                if(recommendedBy == null)
                {
                    error();

                    if(GetString("Deseja tentar novamente? (S - Sim)") == "S") 
                        continue;
                }

                break;
            }
            return recommendedBy;
        }

        static Entity GetContactIDByName(IOrganizationService service, string firstname, string lastname)
        {
            var query = new QueryByAttribute("contact")
            { 
                ColumnSet = new ColumnSet("contactid")
            };

            query.AddAttributeValue("firstname", firstname);
            query.AddAttributeValue("lastname", lastname);

            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        static void CreateAccount(IOrganizationService service)
        {
            Entity recommendedBy = null;

            var accountName = GetString("Por favor, informe o nome da Conta que deseja criar:");

            var monthlySalary = GetFloat("Por favor, informe o seu salario mensal:");

            var age = GetInt("Por favor, informe a sua idade:");

            var accountType = GetAccountType("Por favor, informe o tipo da sua conta (1 - Empresarial / 2 - Pessoal): ");

            string aux = GetString("Deseja indicar o nome de quem te indicou os nossos servicos (S - Sim)? (apenas caso essa pessoa ja esteja nos nossos contatos)");

            if (aux == "S")
            {
                recommendedBy = GetRecommendedBy(service);
            }

            Entity contact = CreateContactEntity(service);

            Entity account = new Entity("account");

            account["name"] = accountName;

            Guid primaryContactId = service.Create(contact);
            Guid recommendedById = new Guid(recommendedBy["contactid"].ToString());

            if (contact != null)
                account["primarycontactid"] = new EntityReference("contact", primaryContactId);

            if (recommendedBy != null)
                account["cpub_recommendedby"] = new EntityReference("contact", recommendedById);

            account["cpub_monthlysalary"] = monthlySalary;
            account["cpub_age"] = age;
            account["cpub_accounttype"] = accountType;

            service.Create(account);
        }

        static Entity CreateContactEntity(IOrganizationService service)
        {
            Entity result = null;

            while(true)
            {
                string aux = GetString("Voce deseja criar um contato para essa conta? (S/N)");

                if (aux == "S")
                    break;
                else if (aux == "N")
                    return result;
                
                error();
            }
          
            string first_name = GetString("Por favor, informe o seu primeiro nome:");
       
            string last_name = GetString("Por favor, informe o seu ultimo nome:");

            string email = GetString("Por favor, informe seu e-mail:");

            string phone = GetString("Por favor, informe seu telefone residencial:");

            Entity contact = new Entity("contact");

            contact["firstname"] = first_name;
            contact["lastname"] = last_name;
            contact["emailaddress1"] = email;
            contact["telephone2"] = phone;
            
            return contact;    
        }

        static void Main(string[] args)
        {
            string aux;

            IOrganizationService service = ConnectionFactory.GetService();

            while (true)
            {
                CreateAccount(service);

                aux = GetString("Deseja cadastrar uma nova conta? (S - Sim)");

                if (aux == "S")
                    continue;
                
                break;
            }

        }
    }
}
