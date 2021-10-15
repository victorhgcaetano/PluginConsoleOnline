using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuglinPortalOnline
{
    public class EventCustom : IPlugin
    {  
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity evento = GetEventEntity(context);

            try
            {                
                if (evento.Contains("g07_eventoid"))
                {
                    //UpdateEvent(context, service, evento);
                    evento["g07_numerodoevento"] = GetNumeroEvento(service) + 1;
                }
                else
                {
                    throw new InvalidPluginExecutionException($"Por favor preencha a conta da oportunidade");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }

        private static int GetNumeroEvento(IOrganizationService service)
        {            
            QueryExpression query = new QueryExpression("g07_evento");
            query.AddOrder("createdon", OrderType.Descending);
            query.TopCount = 1;
            query.ColumnSet.AddColumns("g07_numerodoevento");
            EntityCollection retrieveDeEventos = service.RetrieveMultiple(query);

            return retrieveDeEventos[0].Contains("g07_numerodoevento") ? (int)retrieveDeEventos[0]["g07_numerodoevento"] : 0;            
        }

        private static Entity GetEventEntity(IPluginExecutionContext context)
        {
            Entity evento = new Entity();

            if (context.MessageName == "Create" || context.MessageName == "Update")
            {
                evento = (Entity)context.InputParameters["Target"];
            }
            else
            {
                if (context.MessageName == "Delete")
                {
                    evento = (Entity)context.PreEntityImages["PreImage"];
                }
            }
            return evento;
        }
    }
}
