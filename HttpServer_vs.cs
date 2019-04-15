using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

/// <summary>
/// vs工程
/// </summary>
namespace ServerHttp
{
	class HttpServer
	{
		public HttpListener listener;
		public string ip;
		public string port = "5055";
		public bool isAuto = false;
		public HttpServer()
		{
			ReadXML();
			//IPAddress[] ipaddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

			//for (int i = 0; i < ipaddress.Length; i++)
			//{
			//	if (ipaddress[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			//	{
			//		ip = ipaddress[i].ToString();
			//		break;
			//	}
			//}

			listener = new HttpListener();
			string address = "http://"+ ip+ ":" + port.ToString() +"/";
			Console.WriteLine(address);
			listener.Prefixes.Add(address);

			listener.Start();
			Console.WriteLine("启动服务器");
			listener.BeginGetContext(ListenCB, null);

			
		}
		private void ReadXML()
		{
		

			XmlDocument doc = new XmlDocument();
			XmlReaderSettings setting = new XmlReaderSettings();
			//忽略注释
			setting.IgnoreComments = true;
			//忽略空白
			setting.IgnoreWhitespace = true;
			XmlReader reader = XmlReader.Create("config.xml", setting);
			doc.Load(reader);


			//获取根节点
			XmlNode rootNode = doc.SelectSingleNode("conf/Auto");
			isAuto = rootNode.InnerText == "0" ? false : true;

			 rootNode = doc.SelectSingleNode("conf/IPAddress");
			ip = rootNode.InnerText;
			 rootNode = doc.SelectSingleNode("conf/Port");
			port = rootNode.InnerText;
		}
		private void ListenCB(IAsyncResult ar)
		{
			//Debug.Log("收到请求");
			HttpListenerContext ctx = listener.EndGetContext(ar);
			ctx.Response.StatusCode = 200;
			if (ctx.Request.HttpMethod == "GET")
			{
				GetHandle(ctx);
			}
			if (ctx.Request.HttpMethod == "POST")
			{
				PostHandle(ctx);
			}
			listener.BeginGetContext(ListenCB, null);
		}
		//GET请求
		private void GetHandle(HttpListenerContext ctx)
		{

			//获取RawUrl
			string rawUrl = Path.GetFileName(ctx.Request.RawUrl);
			//接收Get参数
			string username = ctx.Request.QueryString["username"];
			//接收Get参数
			string pwd = ctx.Request.QueryString["pwd"];
			//避免中文乱码进行处理
			//string userName = HttpUtility.ParseQueryString(filename).Get("userName");

			//使用Writer输出http响应代码,UTF8格式
			using (StreamWriter sw = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
			{
				sw.WriteLine("GET Request  Time is:" + DateTime.Now);
				sw.Flush();
				sw.Close();
				sw.Dispose();
			}
			Console.WriteLine("RawUrl:" + rawUrl);
		}

		private void PostHandle(HttpListenerContext ctx)
		{
			//获取Post传递过来的数据
			Stream stream = ctx.Request.InputStream;
			StreamReader reader = new StreamReader(stream, Encoding.UTF8);
			string body = reader.ReadToEnd();

			string username = DecodePost(body, "username");
			string pwd = DecodePost(body, "pwd");
			//C#4.0以上支持的方法
			Console.WriteLine("解析:" + HttpUtility.ParseQueryString(body).Get("userName"));

			//使用Writer输出http响应代码,UTF8格式
			using (StreamWriter sw = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
			{
				sw.WriteLine("POST Request  Time is:" + DateTime.Now);
				sw.Flush();
				sw.Close();
				sw.Dispose();
			}
			Console.WriteLine("Body:" + body);
		}
		private string DecodePost(string postData, string key)
		{
			Dictionary<string, string> returnData = new Dictionary<string, string>();
			string[] allSlice = postData.Split('&');
			for (int i = 0; i < allSlice.Length; i++)
			{
				returnData.Add(allSlice[i].Split('=')[0], allSlice[i].Split('=')[1]);
			}
			if (returnData.ContainsKey(key))
			{
				return returnData[key];
			}
			return null;
		}
		~ HttpServer()
		{

			listener.Close();
		}
	}
}
