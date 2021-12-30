using System;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Yorozu.GoogleDriveHelper
{
	public static class UnityWebRequestExtension
	{
		private static RequestComponent _requestComponent;
		
		/// <summary>
		/// Json を追加
		/// </summary>
		internal static void AddJson(this UnityWebRequest self, string json)
		{
			var postData = Encoding.UTF8.GetBytes(json);
			self.uploadHandler = new UploadHandlerRaw(postData);
			self.downloadHandler = new DownloadHandlerBuffer();
			self.SetRequestHeader("Content-Type", "application/json");
		}

		internal static void SetAccessToken(this UnityWebRequest self, string accessToken)
		{
			if (!string.IsNullOrEmpty(accessToken))
				self.SetRequestHeader("Authorization", string.Format("Bearer {0}", accessToken));
		}

		/// <summary>
		/// Request処理を行う
		/// 動作中ならば専用のオブジェクトを生成する
		/// </summary>
		internal static void Request(this UnityWebRequest self, Action success, Action<ErrorMessage> error)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				self.RequestEditor(success, error);
				return;
			}
#endif
			if (_requestComponent == null)
			{
				var obj = new GameObject("RequestObject", typeof(RequestComponent))
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				
				_requestComponent = obj.GetComponent<RequestComponent>();
			}
			
			_requestComponent.StartCoroutine(RequestImpl(self, success, error));
		}

		private static IEnumerator RequestImpl(UnityWebRequest request, Action success, Action<ErrorMessage> error)
		{
			yield return request.SendWebRequest();

			CheckError(request, success, error);
		}

#if UNITY_EDITOR

		private static void RequestEditor(this UnityWebRequest self, Action success, Action<ErrorMessage> error)
		{
			void Update()
			{
				if (self.result == UnityWebRequest.Result.InProgress)
					return;

				EditorApplication.update -= Update;
				CheckError(self, success, error);
			}

			self.SendWebRequest();

			EditorApplication.update += Update;
		}
#endif

		private static void CheckError(UnityWebRequest request, Action success, Action<ErrorMessage> error)
		{
			if (request.result == UnityWebRequest.Result.Success)
			{
				success?.Invoke();
				return;
			}

			try
			{
				var errorMessage = JsonUtility.FromJson<ErrorMessage>(request.downloadHandler.text);
				error?.Invoke(errorMessage);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(request.downloadHandler.text);
				error?.Invoke(new ErrorMessage());
			}
		}
	}
}
