using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateUserWindow : MonoBehaviour
{
	[SerializeField] InputField name;

	public void CreateUser()
	{
		PlayfabManager.Instance.CreateUser(name.text, () => Hide());
	}

	public void QuitGame() => Application.Quit();

	public void Show() => gameObject.SetActive(true);

	public void Hide() => gameObject.SetActive(false);
}
