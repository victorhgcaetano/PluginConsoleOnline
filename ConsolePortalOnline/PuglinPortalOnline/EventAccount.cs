using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuglinPortalOnline
{
    public class EventAccount : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            Entity conviteEvento = GetConviteEventoEntity(context);

            try
            {
                UpdateAccount(context, service, conviteEvento);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.ToString());
            }
        }

        private static void UpdateAccount(IPluginExecutionContext context, IOrganizationService service, Entity conviteEvento)
        {
            Guid contaDoEvento = ((EntityReference)conviteEvento["g07_cliente"]).Id;

            Entity account = RetrieveAccount(service, contaDoEvento);
            //string ultimoEvento = GetUltimoEvento(service, contaDoEvento, conviteEvento);

            if (context.MessageName == "Create")
            {
                account["g07_ultimoevento"] = ((EntityReference)conviteEvento["g07_evento"]);
                service.Update(account);
            }
            else
            {
                if (context.MessageName == "Delete")
                {
                    account["g07_ultimoevento"] = null;
                }
            }
        }

        private static Entity RetrieveAccount(IOrganizationService service, Guid contaDoEvento)
        {
            return service.Retrieve("account", contaDoEvento, new ColumnSet("g07_ultimoevento"));
        }

        private static string GetUltimoEvento(IOrganizationService service, Guid contaDoEvento, Entity conviteEvento)
        {
            return conviteEvento.Contains("g07_evento") ? ((EntityReference)conviteEvento["g07_evento"]).Name.ToString() : "";
        }

        private static Entity GetConviteEventoEntity(IPluginExecutionContext context)
        {
            Entity conviteEvento = new Entity();

            if (context.MessageName == "Create")
            {
                conviteEvento = (Entity)context.InputParameters["Target"];
            }
            else
            {
                if (context.MessageName == "Delete")
                {
                    conviteEvento = (Entity)context.PreEntityImages["PreImage"];
                }
            }
            return conviteEvento;
        }
    }
}
