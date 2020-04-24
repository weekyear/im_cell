using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private static List<string> Contents;

    [SerializeField] private int ChestIndex;

	private void Start()
    {
        if (ChestIndex != -1)
        {
            gameObject.GetComponent<Animator>().SetBool("IsOpen", GameManager.IsOpenChestList[ChestIndex]);
            var content = Lean.Localization.LeanLocalization.GetTranslationText($"Chest");
            Contents = content.Split('\n').ToList();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision) && !gameObject.GetComponent<Animator>().GetBool("IsOpen"))
        {
            StartCoroutine(OpenChest());
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        return collision.CompareTag("Player");
    }

    private IEnumerator OpenChest()
    {
        gameObject.GetComponent<Animator>().SetBool("IsOpen", true);
        AudioManager.Instance.PlayEffectSound("openchest");
        yield return new WaitForSeconds(1.25f);

        PlayerPrefs.SetInt($"Chest{ChestIndex}", 1);
        if (ChestIndex != -1)
        {
            PlayerObserver.ChestOpened(Contents[ChestIndex]);
            GameManager.IsOpenChestList[ChestIndex] = true;
        }
        else
        {
            var lastMessage = "XXX은 제자리에 가만 있지 못 한다. 본래 해야할 일을 인식하지 못 하고 이곳 저곳에 돌아다니면서 세상을 흰색으로 물들인다.\n" +
                "이윽고 빠르게 세상의 온기를 빼앗으며 더더욱 증식속도를 높인다.\n" +
                "마침내 이 세상을 모두 하얗게 뒤덮는 순간 자신을 위장하던 색을 버리고 이 세상을 물들여버린 흰색으로 그 모습을 물들인다.\n" +
                "XXX은 암세포다.";
            PlayerObserver.ChestOpened(lastMessage);
            AudioManager.Instance.StopBgm();
        }
    }
}
