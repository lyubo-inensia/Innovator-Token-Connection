using Aras.IOM;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace Inensia.IOM
{

    public class TokenHttpConnection : IServerConnection
    {
        public TokenHttpConnection(string token, string innovatorUrl = "http://localhost/InnovatorServer12SP09")
        {
            SessionCache = new ConcurrentDictionary<string, SessionInfo>();
            //token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjY0RjREMUZCMzhGQkE1RTM0NTk0ODY1NjgyMjI1QURCNzk3RjAyRTMiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJaUFRSLXpqN3BlTkZsSVpXZ2lKYTIzbF9BdU0ifQ.eyJuYmYiOjE2MTI4NjU3NzIsImV4cCI6MTYxMjg2OTM3MiwiaXNzIjoiT0F1dGhTZXJ2ZXIiLCJhdWQiOlsiT0F1dGhTZXJ2ZXIvcmVzb3VyY2VzIiwiSW5ub3ZhdG9yIl0sImNsaWVudF9pZCI6IklPTUFwcCIsInN1YiI6ImFkbWluIiwiYXV0aF90aW1lIjoxNjEyODY1NzcyLCJpZHAiOiJsb2NhbCIsInVzZXJuYW1lIjoiYWRtaW4iLCJkYXRhYmFzZSI6IjEyU1AwMV8wMSIsInNjb3BlIjpbIklubm92YXRvciJdLCJhbXIiOlsicGFzc3dvcmQiXX0.QJoY-8hP-pGYk3TpXBURo0e5ubFqIR_JxHlbQv6VBOI72bYyvpwjSLwdkZXHj12KqxLmJUYUqt7oC9vt42r6TxXI9AVGfiN-UkUU_Xi-UZxxub89RO2SApLWVaKGfOLyW_nEyOIOSeiNE46N9paq9uLuPQQKOGvAO9QoQTklUkrp1Bb5WYVEKHOTy0UARUnKDz8a2SamW8WMiN3DQ6CLg8wpThi01mygftqhjBLmbcn-KtFkbtRM6sEFOXsYMnN6eyJwGF6H9hXP3y7E0kdpUslh0DdGxd5BtdYu0LC643VaTkIaa9bdd68DUKNPp-ZEk45036ykv4XBHUnyCYz8jA";
            Token = Regex.Replace(token, "bearer", "", RegexOptions.IgnoreCase).Trim();
            InnovatorUrl = innovatorUrl.EndsWith("/") ? innovatorUrl.Substring(0, innovatorUrl.Length - 1) : innovatorUrl;
            var sessionInfo = GetSessionInfo();
            DbName = sessionInfo?.DbName ?? "";
            UserId = sessionInfo?.UserId ?? "";
            LicenseInfo = sessionInfo?.LicenseInfo ?? "";
        }
        public string InnovatorUrl { get; }
        protected string UserId { get; }
        protected string Token { get; }
        protected string DbName { get; }
        protected string LicenseInfo { get; }
        protected static ConcurrentDictionary<string, SessionInfo> SessionCache { get; set; }

        protected void SetAuthHeader(WebClient client)
        {
            client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);
        }
        
        protected void SaveTokenCache(string token, SessionInfo sessionInfo)
        {
            if (!SessionCache.ContainsKey(Token) && sessionInfo != null)
            {
                SessionCache.TryAdd(token, sessionInfo);
            }
        }
        protected SessionInfo GetTokenCache(string token)
        {
            SessionInfo ret = null;
            if (SessionCache.ContainsKey(token))
            {
                ret = SessionCache[token];
            }

            return ret;
        }
        protected SessionInfo LoadSessionInfo()
        {
            SessionInfo ret = null;
            try
            {
                XmlDocument item = new XmlDocument();
                XmlDocument outItem = new XmlDocument();
                item.LoadXml("<Item type=\"Method\" action=\"get_innovator_session_info\"></Item>");
                CallAction("ApplyMethod", item, outItem);
                var userId = outItem.SelectSingleNode("//Item[@type=\"User\"]").Attributes["id"].Value;
                var dbName = outItem.SelectSingleNode("//db_name").InnerText.Trim();
                var licInfo = outItem.SelectSingleNode("//license_info").InnerText.Trim();

                ret = new SessionInfo
                {
                    DbName = dbName,
                    UserId = userId,
                    LicenseInfo = licInfo
                };
                SaveTokenCache(Token, ret);
            }
            catch (Exception ex)
            {
            }

            return ret;
        }
        protected SessionInfo GetSessionInfo()
        {
            SessionInfo ret = GetTokenCache(Token);
            if (ret == null)
            {
                ret = LoadSessionInfo();
            }

            return ret;
        }

        #region IServerConnection interface implementation
        public void CallAction(string actionName, XmlDocument inDom, XmlDocument outDom)
        {
            var http = new WebClient();
            http.Headers.Add("SOAPAction", actionName);
            SetAuthHeader(http);
            string xml = inDom.OuterXml;
            var url = InnovatorUrl + "/Server/InnovatorServer.aspx";
            var res = http.UploadString(url, "post", $"{xml}");
            outDom.LoadXml(res);
        }

        public void DebugLog(string reason, object msg)
        {

        }

        public bool DebugLogP()
        {
            return true;
        }

        public void DownloadFile(Item fileItem, string directoryPath, bool overwriteFile)
        {

        }

        public string GetDatabaseName()
        {
            return DbName;
        }

        public string[] GetDatabases()
        {
            return new string[] { DbName };
        }

        public string getFileUrl(string fileId, UrlType type)
        {
            return null;
        }

        public ArrayList getFileUrls(ArrayList fileIds, UrlType type)
        {
            return new ArrayList();
        }

        public object GetFromCache(string key)
        {
            return null;
        }

        public string GetLicenseInfo()
        {
            return "";
        }

        public string GetLicenseInfo(string issuer, string addonName)
        {
            return LicenseInfo;
        }

        public object GetOperatingParameter(string name, object defaultvalue)
        {
            return defaultvalue;
        }

        public object GetSrvContext()
        {
            throw new System.NotImplementedException();
        }

        public string getUserID()
        {
            return UserId;
        }

        public string GetValidateUserXmlResult()
        {
            throw new System.NotImplementedException();
        }

        public void InsertIntoCache(string key, object value, string path)
        {

        }
        #endregion
    }
}
