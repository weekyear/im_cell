using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{
	[SerializeField] InputField name;

	private void OnEnable()
	{
		name.text = PlayerPrefs.GetString("TesterId", "");
	}

	public void Login()
	{
		PlayfabManager.Instance.PCLogin(name.text, () => Hide());
	}

	public void QuitGame() => Application.Quit();

	public void Show() => gameObject.SetActive(true);

	public void Hide() => gameObject.SetActive(false);
}
