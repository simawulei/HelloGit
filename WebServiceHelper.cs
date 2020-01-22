using System;
using System.Net;
using System.IO;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using System.Web.Services.Protocols;  

namespace WebServiceModel
{
    /* ���÷�ʽ   
     *   string url = "http://www.webservicex.net/globalweather.asmx" ;   
     *   string[] args = new string[2] ;   
     *   args[0] = "Hangzhou";   
     *   args[1] = "China" ;   
     *   object result = WebServiceHelper.InvokeWebService(url ,"GetWeather" ,args) ;   
     *   Response.Write(result.ToString());   
     */   
    public class WebServiceHelper
    {
        #region InvokeWebService
        /// < summary>   
        /// ��̬����web����   
        /// < /summary>   
        /// < param name="url">WSDL�����ַ< /param>   
        /// < param name="methodname">������< /param>   
        /// < param name="args">����< /param>   
        /// < returns>< /returns>   
        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return WebServiceHelper.InvokeWebService(url, null, methodname, args);
        }

        /// < summary>   
        /// ��̬����web����   
        /// < /summary>   
        /// < param name="url">WSDL�����ַ< /param>   
        /// < param name="classname">����< /param>   
        /// < param name="methodname">������< /param>   
        /// < param name="args">����< /param>   
        /// < returns>< /returns>   
        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                classname = WebServiceHelper.GetWsClassName(url);
            }

            try
            {
                //��ȡWSDL   
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //���ɿͻ��˴��������   
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider icc = new CSharpCodeProvider();

                //�趨�������   
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //���������   
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //���ɴ���ʵ���������÷���   
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);

                return mi.Invoke(obj, args);

                /*   
                PropertyInfo propertyInfo = type.GetProperty(propertyname);   
                return propertyInfo.GetValue(obj, null);   
                */
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }

        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        }
        #endregion
    }
}
