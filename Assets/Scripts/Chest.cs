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
        var content = Lean.Localization.LeanLocalization.GetTranslationText($"Chest");
        Contents = content.Split('\n').ToList();
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
        GameManager.EffectAudio.PlayEffectSound("openchest");
        yield return new WaitForSeconds(1.25f);

        PlayerObserver.ChestOpened(Contents[ChestIndex]);
    }
}
