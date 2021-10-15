using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolePortalOnline.Model
{
    public class Opportunity
    {
        public string TableName = "opportunity";
        public IOrganizationService Service { get; set; }

        public Opportunity(IOrganizationService service)
        {
            this.Service = service;
        }

        public EntityCollection RetrieveAccountByOpportunity(Guid opportunityId)
        {
            QueryExpression queryOpportunitys = new QueryExpression(this.TableName);
            queryOpportunitys.ColumnSet.AddColumns("parentaccountid", "name", "totalamount");
            queryOpportunitys.Criteria.AddCondition("opportunityid", ConditionOperator.Equal, opportunityId);

            queryOpportunitys.AddLink("account", "parentaccountid", "accountid", JoinOperator.Inner);
            queryOpportunitys.LinkEntities[0].Columns.AddColumns("g07_niveldocliente");
            queryOpportunitys.LinkEntities[0].EntityAlias = "conta";

            return this.Service.RetrieveMultiple(queryOpportunitys);
        }


        public EntityCollection RetrieveNivelClienteByOpportunityAccount(Guid opportunityAccountId)
        {
            QueryExpression queryAccounts = new QueryExpression("account");
            queryAccounts.ColumnSet.AddColumns("g07_niveldocliente");
            queryAccounts.Criteria.AddCondition("accountid", ConditionOperator.Equal, opportunityAccountId);

            queryAccounts.AddLink("g07_niveldocliente", "g07_niveldocliente", "g07_niveldoclienteid", JoinOperator.Inner);
            queryAccounts.LinkEntities[0].Columns.AddColumns("g07_porcentagemdedesconto", "g07_valordedesconto");
            queryAccounts.LinkEntities[0].EntityAlias = "nivelCliente";

            return this.Service.RetrieveMultiple(queryAccounts);
        }

        public void UpdateOpportunity(Guid opportunityId, decimal valorDesconto)
        {
            Entity opportunity = new Entity(this.TableName);
            opportunity.Id = opportunityId;
            opportunity["discountamount"] = new Money(valorDesconto);
            this.Service.Update(opportunity);
        }
    }
}
