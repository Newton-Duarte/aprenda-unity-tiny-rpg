using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    NPC npc;
    public GameObject gNpc;
    public GameObject dialogPanel;
    public Text personNameText;
    public Text speechText;

    public Queue<string> speeches;

    [Header("Interactive System")]
    public Text txtQuestionNpcName;
    public GameObject questionPanel;
    public Text txtQuestion;
    public Button[] btnAlternatives;
    public Text[] txtAlternative;

    // Start is called before the first frame update
    void Start()
    {
        dialogPanel.SetActive(false);

        speeches = new Queue<string>();
    }

    public void nextSpeech()
    {
        StopCoroutine(typeText(""));

        if (speeches.Count == 0)
        {
            //endConversation();
            gNpc.SendMessage("endConversation", SendMessageOptions.DontRequireReceiver);
            return;
        }

        string speech = speeches.Dequeue();
        StartCoroutine(typeText(speech));
    }

    IEnumerator typeText(string text)
    {
        speechText.text = "";
        foreach(char letter in text.ToCharArray())
        {
            speechText.text += letter.ToString();
            yield return new WaitForEndOfFrame();
        }
    }

    public void respond(int i)
    {
        questionPanel.SetActive(false);
        gNpc.SendMessage("respondQuestion", i, SendMessageOptions.DontRequireReceiver);
    }
}
