using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Net;
using System.Threading;

public class UnityHttpListener : MonoBehaviour
{
	[SerializeField]
	private string httpPort = "4444";

	[SerializeField]
	private TextAsset htmlResponseFile;

	private HttpListener listener;
	private Thread listenerThread;

	string htmlString = "";

	void Start()
	{
		StartListening();
	}

	public void StartListening()
    {
		listener = new HttpListener();
		listener.Prefixes.Add("http://localhost:" + httpPort + "/");
		listener.Prefixes.Add("http://127.0.0.1:" + httpPort + "/");
		listener.Prefixes.Add("http://" + IPLogger.GetLocalIPv4() + ":" + httpPort + "/");
		listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		listener.Start();

		listenerThread = new Thread(startListener);
		listenerThread.Start();
		Debug.Log("Server Started @ " + IPLogger.GetLocalIPv4() + ":" + httpPort + "/");

		if (htmlResponseFile != null)
		{
			Debug.Log("Preparing to send:");
			Debug.Log(htmlResponseFile.text);
			htmlString = htmlResponseFile.text;
		}
	}

	public void StopListening()
    {
		listener.Stop();
    }

	private void startListener()
	{
		while (true)
		{
			var result = listener.BeginGetContext(ListenerCallback, listener);
			result.AsyncWaitHandle.WaitOne();
		}
	}

	private void ListenerCallback(IAsyncResult result)
	{
		var context = listener.EndGetContext(result);

		Debug.Log("Method: " + context.Request.HttpMethod);
		Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

		// Obtain a response object.
		HttpListenerResponse response = context.Response;

		// Construct a response.
		string responseString = "<HTML><BODY> Hello from the string!</BODY></HTML>";
		if (htmlResponseFile != null)
		{
			responseString = htmlString;
		}
		byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		// Get a response stream and write the response to it.
		response.ContentLength64 = buffer.Length;
		System.IO.Stream output = response.OutputStream;
		output.Write(buffer, 0, buffer.Length);

		if (context.Request.QueryString.AllKeys.Length > 0)
			foreach (var key in context.Request.QueryString.AllKeys)
			{
				Debug.Log("Key: " + key + ", Value: " + context.Request.QueryString.GetValues(key)[0]);
			}

		if (context.Request.HttpMethod == "POST")
		{
			Thread.Sleep(1000);
			var data_text = new StreamReader(context.Request.InputStream,
								context.Request.ContentEncoding).ReadToEnd();
			Debug.Log(data_text);
		}

		context.Response.Close();
	}

}
