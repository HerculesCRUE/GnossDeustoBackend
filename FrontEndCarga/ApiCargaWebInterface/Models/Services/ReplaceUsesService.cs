using ApiCargaWebInterface.Utility;
using ApiCargaWebInterface.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCargaWebInterface.Models.Services
{
    public class ReplaceUsesService
    {
        public static CmsDataViewModel PageWithDirectives(string htmlContent, CmsDataViewModel dataModel)
        {
            
            if (htmlContent.Contains(DirectivesList.StatisticsGrafo))
            {
                htmlContent = htmlContent.Replace(DirectivesList.StatisticsGrafo, StatisticsGrafo());
            }
            if (htmlContent.Contains(DirectivesList.StatisticsTriples))
            {
                htmlContent = htmlContent.Replace(DirectivesList.StatisticsGrafo, StatisticsTriples());
            }
            if (htmlContent.Contains(DirectivesList.StatisticsEntidades))
            {
                htmlContent = htmlContent.Replace(DirectivesList.StatisticsGrafo, StatisticsEntidades());
            }
            if (htmlContent.Contains(DirectivesList.Sparql))
            {
                htmlContent = htmlContent.Replace(DirectivesList.StatisticsGrafo, Sparql());
            }
            if (htmlContent.Contains(DirectivesList.DirectivaJavi))
            {
                dataModel = DirectivaJavi(dataModel);
            }
            return dataModel;
        }

        private static string StatisticsGrafo()
        {
            string content = "";
            return content;
        }

        private static string StatisticsTriples()
        {
            string content = "";
            return content;
        }

        private static string StatisticsEntidades()
        {
            string content = "";
            return content;
        }

        private static string Sparql()
        {
            string content = "";
            return content;
        }

        private static CmsDataViewModel DirectivaJavi(CmsDataViewModel dataModel)
        {
            dataModel.Name = "Javier";
            dataModel.LastName = "Ruiz Sáenz de Pipaón";
            return dataModel;
        }
    }
}
