using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestInfo : MonoBehaviour {
    
    private Quest quest;
    private Canvas canvas;

    public Text textName;
    public Image icon;
    public Text descriptionText;
    public Button btn;
    public Text btnText;

    public void SetQuestDisplay(Quest q, Canvas c){
        quest = q;
        canvas = c;

        textName.name = quest.name;
        icon.sprite = quest.icon;

        float height = 90f;
        string s = "";
        if ( quest.questType == QuestType.main ){
            s += "Main\n";
        } else if ( quest.questType == QuestType.side ){
            s += "Side\n";
        } else {
            s += "Event\n";
        }
        s += quest.description + "\n";
        
        for (int i = 0; i < quest.questObjectives.Length; i++){
            s += quest.questObjectives[i].objective + " " + quest.questObjectives[i].amt + " " + quest.questObjectives[i].GetObjectiveName();
            height += 30f;
        }

        descriptionText.text = s;
        RectTransform rt = descriptionText.transform.parent as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x,height);
    }

    // @param2: True if display is from player's quest window
    public void SetQuest(Quest q, Canvas c, bool b){
        quest = q;
        canvas = c;

        textName.text = quest.name;
        icon.sprite = quest.icon;

        float height = 90f;
        string s = "";
        if ( quest.questType == QuestType.main ){
            s += "Main\n";
        } else if ( quest.questType == QuestType.side ){
            s += "Side\n";
        } else {
            s += "Event\n";
        }
        s += quest.description + "\n";
        
        for (int i = 0; i < quest.questObjectives.Length; i++){
            s += quest.questObjectives[i].objective + " " + quest.questObjectives[i].amt + " " + quest.questObjectives[i].GetObjectiveName();
            height += 30f;
        }

        descriptionText.text = s;
        RectTransform rt = descriptionText.transform.parent as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x,height);

        if ( b ){
            if ( Game.GetPlayer().HasQuest(quest.id) ){
                btnText.text = "Abandon";
                btn.onClick.AddListener(() => Abandon());
            } else {
                btnText.text = "Obtain";
                btn.onClick.AddListener(() => Obtain());
            }
        } else {
            Quest qt = Game.GetPlayer().GetQuest(q.id);
            if ( qt != null ){
                if ( qt.IsDone() ){
                    btnText.text = "Complete";
                    btn.onClick.AddListener(() => Complete());
                } else {
                    btnText.text = "In Progress";
                }
            }
        }
    }

    public void Abandon(){
        Quest q = Game.GetPlayer().GetQuest(quest.id);
        if ( q != null ){
            Game.GetPlayer().AbandonQuest(q);
        }
    }

    public void Complete(){
        Quest q = Game.GetPlayer().GetQuest(quest.id);
        if ( q != null && q.IsDone() ){
            Game.GetPlayer().CompleteQuest(q);
            Game.Notification(canvas,"Quest Complete!",true);
            canvas.GetComponent<NPCCanvas>().Quest();
        }
    }

    public void Obtain(){
        if ( quest != null )
            Game.GetPlayer().AddQuest(quest);
    }
}
