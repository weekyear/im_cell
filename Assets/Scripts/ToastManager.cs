using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ToastManager : MonoBehaviour
{

	public static ToastManager Instance { get; private set; }

#if UNITY_ANDROID && !UNITY_EDITOR
	private AndroidJavaObject currentActivity;
	private AndroidJavaClass UnityPlayer;
	private AndroidJavaObject context;
	private AndroidJavaObject toast;
#endif

	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

#if UNITY_ANDROID && !UNITY_EDITOR
		UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

		currentActivity = UnityPlayer
			.GetStatic<AndroidJavaObject>("currentActivity");

		context = currentActivity
			.Call<AndroidJavaObject>("getApplicationContext");
#endif
	}

	public void Show(string message)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		currentActivity.Call
		(
			"runOnUiThread",
			new AndroidJavaRunnable(() =>
			{
				AndroidJavaClass Toast
				= new AndroidJavaClass("android.widget.Toast");

				AndroidJavaObject javaString
				= new AndroidJavaObject("java.lang.String", message);

				toast = Toast.CallStatic<AndroidJavaObject>
				(
					"makeText",
					context,
					javaString,
					Toast.GetStatic<int>("LENGTH_SHORT")
				);

				toast.Call("show");
			})
		 );
#else
		Debug.LogError(message);
#endif
	}
}
