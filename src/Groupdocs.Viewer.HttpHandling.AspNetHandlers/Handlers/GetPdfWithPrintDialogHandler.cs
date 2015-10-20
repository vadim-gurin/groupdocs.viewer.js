﻿using System;
using System.Collections.Specialized;
using System.Web;
using Groupdocs.Web.UI;
using Groupdocs.Web.UI.ViewModels;

namespace Groupdocs.Viewer.HttpHandling.AspNetHandlers.Handlers
{
    public class GetPdfWithPrintDialogHandler : BaseAspNetHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public override bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public override void ProcessRequest(HttpContext context)
        {
            try
            {
                if (!_helper.IsRequestHandlingEnabled(Constants.GroupdocsPrintRequestHandlingIsEnabled))
                    return;

                GetFileViewModel viewModel = new GetFileViewModel();
                NameValueCollection parameters = context.Request.Params;
                viewModel.Path = GetParameter<string>(parameters, "path");
                viewModel.GetPdf = true;
                viewModel.IsPrintable = true;
                viewModel.WatermarkText = GetParameter<string>(parameters, "watermarkText");
                viewModel.WatermarkColor = GetParameter<int?>(parameters, "watermarkColor");
                viewModel.WatermarkPosition = GetParameter<WatermarkPosition>(parameters, "watermarkPosition");
                viewModel.WatermarkWidth = GetParameter<float>(parameters, "watermarkWidth");
                viewModel.IgnoreDocumentAbsence = false;
                viewModel.UseHtmlBasedEngine = GetParameter<bool>(parameters, "useHtmlBasedEngine");
                viewModel.SupportPageRotation = GetParameter<bool>(parameters, "supportPageRotation");
                viewModel.InstanceIdToken = GetParameter<string>(parameters, Constants.InstanceIdRequestKey);

                byte[] bytes;
                string fileDisplayName;
                bool isSuccessful = GetFile(viewModel,
                                         out bytes, out fileDisplayName);
                if (!isSuccessful || bytes == null)
                    return;

                context.Response.ContentType = "application/pdf";
                context.Response.BinaryWrite(bytes);
            }
            catch (Exception exception)
            {
                OnException(exception, context);
            }
        }

        #endregion
    }
}
