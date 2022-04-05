using UnityEngine;
using UnityEngine.UI;

public class JoinScreen: MonoBehaviour
{
    public static JoinScreen instance;
    
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

        Client.instance.Connect();
    }

    public void BackToJoin() {
        usernameField.interactable = true;
        joinUI.SetActive(true);
    }

    public void SendJoin() {
        Packet packet = Packet.Create(ClientToServer.Join);
        packet.AddString(usernameField.text);
        Client.instance.Send(packet);
        print("we sent a join packet");
    }
}
