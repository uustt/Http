using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unity工程
/// </summary>
public class HttpServer : MonoBehaviour
{

	public Text tipInfoText;
	public InputField ip_input;
	public InputField port_input;


	public string ip = "127.0.0.1";
	public int port = 5055;
	public HttpListener listener = new HttpListener();

	// Use this for initialization
	void Start()
	{
		IPAddress[] ipaddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

		for (int i = 0; i < ipaddress.Length; i++)
		{
			if (ipaddress[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			{
				ip = ipaddress[i].ToString();
				break;
			}
		}
		ip_input.text = ip;
		port_input.text = port.ToString();

	}
	public void StartServer()
	{
		listener = new HttpListener();
		listener.Prefixes.Add(string.Format("http://{0}:{1}/", ip, port));

		listener.Start();
		Debug.Log("启动服务器");
		tipInfoText.text = "启动服务器成功";
		listener.BeginGetContext(ListenCB, null);
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
		//Console.WriteLine("解析:" + HttpUtility.ParseQueryString(body).Get("userName"));

		//使用Writer输出http响应代码,UTF8格式
		using (StreamWriter sw = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
		{
			sw.WriteLine("POST Request  Time is:" + DateTime.Now);
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}
	private string DecodePost(string postData, string key)
	{
		Dictionary<string, string> returnData = new Dictionary<string, string>();
		string[] allSlice = postData.Split('&');
		for (int i = 0; i < allSlice.Length; i++)
		{
			returnData.Add(allSlice[i].Split('=')[0], allSlice[i].Split('=')[1]);
		}
		if(returnData.ContainsKey(key))
		{
			return returnData[key];
		}
		return null;
	}
	private void OnDestroy()
	{
		listener.Close();
		Debug.Log("关闭服务器");
	}
}
