using RiptideNetworking;

using UnityEngine;
using UnityEngine.UI;

public enum Activity
{
    None,
    Quiz,
}

public class PlayerActivity : MonoBehaviour
{
    [SerializeField]
    GameObject activityButton;
    [SerializeField]
    Text activityText;
    
    public Activity activity { get; private set; }

    void Awake() {
        activityButton = GameLogic.instance.activityButton;
        activityText = GameLogic.instance.activityText;
    }

    public void SwitchActivity(Activity newActivity) {
        activity = newActivity;
        activityText.text = GameLogic.instance.activityNames[(int) activity];
        
        activityButton.SetActive(activity != Activity.None);
    }

    public void DoActivity() {
        activityButton.SetActive(false);
        
        switch (activity)
        {
        case Activity.Quiz:
            SendDoQuiz();
            break;
        }
    }

    public void SendDoQuiz() {
        Message msg = Message.Create(MessageSendMode.reliable, ClientToServerId.Quiz);
        NetworkManager.instance.client.Send(msg);
    }
}
