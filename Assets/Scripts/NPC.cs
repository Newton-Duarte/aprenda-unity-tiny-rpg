using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class NPC : MonoBehaviour
{
    DialogManager _dialogManager;
    Inventory _inventory;
    GameController _gameController;

    public string xmlNpcName;
    public string xmlName;
    public Dialog dialog;
    public int speechId;
    public List<Dialog> speeches;
    public Dialog lastSpeech;
    public Dialog completedQuest;
    public bool isAllSpeeches;
    public bool isSpeechContinue;
    public bool isShop;

    [Header("Question System")]
    public string npcName;
    public string question;
    public List<string> alternatives;
    public List<string> targetXmls;

    [Header("Quest System")]
    bool[,] quests;
    public Item[] questItems;
    public int[] questItemsQuantity;
    public string[] questCompletedXml;
    bool isEndQuest;
    bool isBackTo;
    int curretQuestId;

    void Start()
    {
        _dialogManager = FindObjectOfType(typeof(DialogManager)) as DialogManager;
        _inventory = FindObjectOfType(typeof(Inventory)) as Inventory;
        _gameController = FindObjectOfType(typeof(GameController)) as GameController;
        loadXMLData(xmlName);
        initializeQuests();
    }

    void initializeQuests()
    {
        quests = new bool[2, 2];
        quests[0, 0] = false; // Indica se quest está ativa
        quests[0, 1] = false; // Indica se quest está completa

        quests[1, 0] = false; // Indica se quest está ativa
        quests[1, 1] = false; // Indica se quest está compelta
    }

    public void nextSpeech()
    {
        if (isEndQuest)
        {
            loadXMLData(questCompletedXml[curretQuestId]);
        }
        else if (!isAllSpeeches)
        {
            speechId++;

            if (speechId >= speeches.Count)
            {
                isAllSpeeches = true;
                dialog = lastSpeech;
                return;
            }

            dialog.personName = speeches[speechId].personName;
            dialog.speeches = speeches[speechId].speeches;
        }
        else if (isAllSpeeches)
        {
            dialog = lastSpeech;
        }

    }

    void loadXMLData(string xml)
    {
        speechId = 0;
        isAllSpeeches = false;
        isEndQuest = false;
        isBackTo = false;
        npcName = "";
        question = "";
        speeches.Clear();
        alternatives.Clear();
        targetXmls.Clear();
        lastSpeech.personName = "";
        lastSpeech.speeches.Clear();
        lastSpeech = new Dialog();

        TextAsset xmlData = (TextAsset)Resources.Load($"NPC/{xmlNpcName}/{xml}");
        XmlDocument xmlDocument = new XmlDocument();

        xmlDocument.LoadXml(xmlData.text);

        if (xmlDocument["npc"].Attributes["quest"] != null)
        {
            int questId = int.Parse(xmlDocument["npc"].Attributes["quest"].Value);
            quests[questId, 0] = true;
        }

        if (xmlDocument["npc"].Attributes["back_to"] != null)
        {
            isBackTo = true;
            xmlName = xmlDocument["npc"].Attributes["back_to"].Value;
        }

        int i = 0;

        foreach (XmlNode node in xmlDocument["npc"].ChildNodes)
        {
            if (node.Name == "dialog")
            {
                speeches.Add(new Dialog());
                string personName = node.Attributes["name"].Value;
                speeches[i].personName = personName;
                speeches[i].speeches = new List<string>();

                foreach(XmlNode n in node["texts"].ChildNodes)
                {
                    speeches[i].speeches.Add(n.InnerText);
                }
                i++;
            }
            else if (node.Name == "end_dialog")
            {
                string personName = node.Attributes["name"].Value;
                lastSpeech.personName = personName;
                lastSpeech.speeches = new List<string>();

                foreach (XmlNode n in node["texts"].ChildNodes)
                {
                    lastSpeech.speeches.Add(n.InnerText);
                }
            }
            else if (node.Name == "quest_dialog")
            {
                completedQuest = new Dialog();
                string personName = node.Attributes["name"].Value;
                completedQuest.personName = personName;
                completedQuest.speeches = new List<string>();

                foreach (XmlNode n in node["texts"].ChildNodes)
                {
                    completedQuest.speeches.Add(n.InnerText);
                }
            }
            else if (node.Name == "question")
            {
                npcName = node.Attributes["name"].Value;
                question = node["texts"].FirstChild.InnerText;
                alternatives = new List<string>();

                targetXmls = new List<string>();

                foreach(XmlNode n in node["alternatives"].ChildNodes)
                {
                    alternatives.Add(n.InnerText);
                    targetXmls.Add(n.Attributes["name"].Value);
                }
            }

        }

        if (!isAllSpeeches)
        {
            dialog.personName = speeches[0].personName;
            dialog.speeches = speeches[0].speeches;
        }
        else
        {
            dialog = lastSpeech;
        }
    }

    public string formatText(string speech)
    {
        string temp = speech;

        temp = temp.Replace("npc_name", dialog.personName);

        temp = temp.Replace("cor_amarelo", "<color=#FFDC00FF>");
        temp = temp.Replace("fim_cor", "</color>");

        return temp;
    }

    public void respondQuestion(int i)
    {
        isAllSpeeches = false;
        loadXMLData(targetXmls[i]);
        startConversation();
    }

    public void startConversation()
    {
        _dialogManager.dialogPanel.SetActive(true);
        _dialogManager.personNameText.text = "";
        _dialogManager.speechText.text = "";
        _dialogManager.speeches.Clear();

        for (int i = 0; i < quests.Length / 2; i++)
        {
            if (quests[i, 0] == true && quests[i, 1] == false)
            {
                curretQuestId = i;

                if (_inventory.isItemInInventory(questItems[i]))
                {
                    if (questItems[i].stackQuantity && _inventory.itemsQuantity[questItems[i].itemId] >= questItemsQuantity[i])
                    {
                        completeQuest();
                    }
                    else if (!questItems[i].stackQuantity)
                    {
                        completeQuest();
                    }
                }
                break;
            }
        }

        foreach (string speech in dialog.speeches)
        {
            _dialogManager.speeches.Enqueue(formatText(speech));
        }

        _dialogManager.personNameText.text = dialog.personName;
        _dialogManager.nextSpeech();
    }

    void completeQuest()
    {
        quests[curretQuestId, 1] = true;
        dialog = completedQuest;
        _inventory.removeItem(questItems[curretQuestId], questItemsQuantity[curretQuestId]);
        isEndQuest = true;
    }

    public void endConversation()
    {
        nextSpeech();

        if (isSpeechContinue && !isAllSpeeches)
        {
            startConversation();
        }
        else
        {
            _dialogManager.dialogPanel.SetActive(false);

            if (isShop)
            {
                _inventory.openShopPanel();
            }
            else if (question != "")
            {
                _dialogManager.txtQuestionNpcName.text = npcName;
                _dialogManager.txtQuestion.text = question;

                int i = 0;
                foreach(string alternative in alternatives)
                {
                    _dialogManager.btnAlternatives[i].gameObject.SetActive(true);
                    _dialogManager.txtAlternative[i].text = alternative;
                    i++;
                }

                _dialogManager.questionPanel.SetActive(true);
                print("Abriu question panel");
            }
            else if (isBackTo)
            {
                loadXMLData(xmlName);
            }
        }

        if (!_dialogManager.dialogPanel.activeSelf && !_dialogManager.questionPanel.activeSelf)
        {
            _gameController.setGameState(GameState.gameplay);
        }
    }
}
