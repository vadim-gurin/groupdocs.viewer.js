﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using Groupdocs.Web.UI;

namespace Groupdocs.Viewer.HttpHandling.AspNetHandlers.Handlers
{
    /// <summary>
    /// Returns information about all files and folders, which can be opened via "Document Browser" button on toolbar, in a form of JSON
    /// </summary>
    public class LoadFileBrowserTreeDataHandler : BaseAspNetHandler
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
                if (!_helper.IsRequestHandlingEnabled(Constants.GroupdocsFileListRequestHandlingIsEnabled))
                    return;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string path = null;
                int pageIndex = 0;
                int pageSize = -1;
                string orderBy = null;
                bool orderAsc = true;
                string filter = null;
                string fileTypes = null;
                bool extended = false;
                string instanceId = null;
                
                string json;
                bool isJsonP = context.Request.HttpMethod == "GET";

                if (isJsonP)
                    json = context.Request.Params["data"];
                else
                    json = new StreamReader(context.Request.InputStream).ReadToEnd();
                Dictionary<string, string> inputParameters = serializer.Deserialize<Dictionary<string, string>>(json);
                GetParameter(inputParameters, "path", ref path);
                GetParameter(inputParameters, "pageIndex", ref pageIndex);
                GetParameter(inputParameters, "pageSize", ref pageSize);
                GetParameter(inputParameters, "orderBy", ref orderBy);
                GetParameter(inputParameters, "orderAsc", ref orderAsc);
                GetParameter(inputParameters, "filter", ref filter);
                GetParameter(inputParameters, "fileTypes", ref fileTypes);
                GetParameter(inputParameters, "extended", ref extended);
                GetParameter(inputParameters, Constants.InstanceIdRequestKey, ref instanceId);
                
                object data = LoadFileBrowserTreeData(path, pageIndex, pageSize, orderBy, orderAsc, filter,
                                                              fileTypes, extended, instanceId);
                if (data == null)
                    return;

                string serializedData = serializer.Serialize(data);
                CreateJsonOrJsonpResponse(context, serializedData);
            }
            catch (Exception exception)
            {
                OnException(exception, context);
            }
        }

        #endregion
    }
}
