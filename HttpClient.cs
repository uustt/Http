using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Unity客户端
/// </summary>
public class HttpClient : MonoBehaviour {

	public void SendRequestGet () {

		StartCoroutine(RequestGet());
	}
	private IEnumerator RequestGet()
	{
		UnityWebRequest request = UnityWebRequest.Get("http://www.sttplay.com:80?password=123456&username=彤彤");
		yield return request.SendWebRequest();
		if(request.error != null)
		{
			Debug.Log(request.error);
			yield break;
		}
		else
		{
			Debug.Log(request.responseCode);
			Debug.Log(request.downloadHandler.text);
		}
	}
	public void SendRequestPost()
	{
		StartCoroutine(RequestPost());
	}
	private IEnumerator RequestPost()
	{
		Dictionary<string, string> postData = new Dictionary<string, string>();
		postData.Add("stt", "1234");
		postData.Add("wxy", "loveyou");
		WWWForm wf = new WWWForm();
		wf.AddField("username", 200);
		wf.AddField("pwd", "qqlove");
		wf.AddField("http", "xxasf");
		wf.AddField("urls", "666");
		UnityWebRequest request = UnityWebRequest.Post("http://192.168.51.217:5055/",wf);
		yield return request.SendWebRequest();
		if (request.error != null)
		{
			Debug.Log(request.error);
			yield break;
		}
		else
		{
			Debug.Log(request.responseCode);
			Debug.Log(request.downloadHandler.text);
		}
	}
}
