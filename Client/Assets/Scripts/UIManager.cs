using RiptideNetworking;

using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager instance {
        get => _instance;
        private set {
            if (_instance is null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.LogWarning($"instance of {nameof(UIManager)} exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    [Header("Connect")]
    [SerializeField]
    GameObject joinUI;
    [SerializeField]
    InputField usernameField;
    
    void Awake() {
        instance = this;
    }

    public void JoinClicked() {
        usernameField.interactable = false;
        joinUI.SetActive(false);

        NetworkManager.instance.Connect();
    }

    public void BackToJoin() {
        usernameField.interactable = true;
        joinUI.SetActive(true);
    }

    public void SendName() {
        Message msg = Message.Create(MessageSendMode.reliable, ClientToServerId.Name);
        msg.AddString(usernameField.text);
        NetworkManager.instance.client.Send(msg);
    }
}
