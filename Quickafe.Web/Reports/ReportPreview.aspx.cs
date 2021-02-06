using Microsoft.Reporting.WebForms;
using Quickafe.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Quickafe.Web.Reports
{
    public partial class ReportPreview : Page
    {
        public ReportProvider ReportProvider => (ReportProvider)Autofac.Integration.Mvc.AutofacDependencyResolver.Current.GetService(typeof(ReportProvider));
        public ConfigurationProvider ConfigurationProvider => (ConfigurationProvider)Autofac.Integration.Mvc.AutofacDependencyResolver.Current.GetService(typeof(ConfigurationProvider));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rptReport.InteractivityPostBackMode = InteractivityPostBackMode.AlwaysAsynchronous;
                string reportName = Request.QueryString["Report"];
                var keys = Request.QueryString.AllKeys.Where(param => param != "Report");

                var parameters = new List<ReportParameter>();
                foreach (string key in keys)
                    parameters.Add(new ReportParameter(key, Request.QueryString[key]));

                ShowReport(reportName, parameters);
            }
        }

        public void ShowReport(string reportName, List<ReportParameter> parameters)
        {
            rptReport.Visible = true;
            rptReport.ProcessingMode = ProcessingMode.Remote;
            rptReport.ServerReport.ReportServerUrl = new Uri(ConfigurationProvider[ConfigurationKeys.ReportServerUrl]);
            rptReport.ServerReport.ReportPath = String.Format(@"/{0}/{1}", ConfigurationProvider[ConfigurationKeys.ReportFolder], reportName);
            rptReport.ServerReport.SetParameters(parameters);
            rptReport.ServerReport.Refresh();
        }

    }
}