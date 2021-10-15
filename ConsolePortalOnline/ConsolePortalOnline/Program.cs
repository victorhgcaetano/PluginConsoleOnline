using ConsoleIntegracao;
using ConsolePortalOnline.Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePortalOnline
{
    public class Program
    {
        static void Main(string[] args)
        {
            IOrganizationService service = ConnectionFactory.GetCrmService();
            Opportunity opportunity = new Opportunity(service);
            ControlarCadastroDesconto(opportunity);
        }

        private static void ControlarCadastroDesconto(Opportunity opportunity)
        {
            bool ok = false;
            string leituraDoUsuario;
            decimal valorDesconto = 0;
            Guid oportunidadeID = new Guid();
            EntityCollection opportunitysCRM;

            do
            {
                Console.WriteLine("Qual oportunidade você deseja aplicar o desconto ? ");
                leituraDoUsuario = Console.ReadLine();
                oportunidadeID = new Guid(leituraDoUsuario);
                try
                {
                    opportunitysCRM = opportunity.RetrieveAccountByOpportunity(oportunidadeID);
                    if (opportunitysCRM.Entities.Count() > 0)
                    {
                        ok = true;
                        foreach (Entity opportunityCRM in opportunitysCRM.Entities)
                        {
                            Money valorTotal = (Money) opportunityCRM["totalamount"];
                            EntityReference parentAccountId = (EntityReference)opportunityCRM["parentaccountid"];                                              

                            EntityReference nivelCliente = (EntityReference)((AliasedValue)opportunityCRM["conta.g07_niveldocliente"]).Value;                                                      

                            EntityCollection accountsCRM = opportunity.RetrieveNivelClienteByOpportunityAccount(new Guid(parentAccountId.Id.ToString()));

                            foreach (Entity accountCRM in accountsCRM.Entities)
                            {                                
                                string percentualDesconto = (string)((AliasedValue)accountCRM["nivelCliente.g07_valordedesconto"]).Value;
                                percentualDesconto = percentualDesconto.Substring(0, percentualDesconto.IndexOf('%')).Trim();
                                valorDesconto = Math.Round(Convert.ToDecimal(valorTotal.Value) * (Convert.ToDecimal(percentualDesconto) / 100),2);                                
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Não foi localizada esta oportunidade! ");
                    }
                }
                catch
                {
                    Console.WriteLine("Você digitou um valor inválido! ");
                }
            } while (!ok);

            Console.WriteLine($"O valor do desconto para esta oportunidade é : {valorDesconto}");

            ok = false;
            do
            {
                Console.WriteLine("Você deseja atualizar essa oportunidade? (Y/N) ");
                leituraDoUsuario = Console.ReadLine().ToUpper();

                if (leituraDoUsuario == "Y")
                {
                    opportunity.UpdateOpportunity(oportunidadeID, valorDesconto);
                    ok = true;
                    Console.WriteLine("Sucesso! Desconto atualizado nesta oportunidade.");
                }
                else if (leituraDoUsuario == "N")
                {
                    ok = true;
                    Console.WriteLine("A oportunidade não foi atualizada. Obrigado!");
                }
                else
                {
                    Console.WriteLine("Você digitou um valor inválido! ");
                }
            } while (!ok);
            Console.ReadKey();
        }

    }
}
