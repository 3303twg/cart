using Photon.Pun; // ����Ƽ�� ���� ������Ʈ��
using Photon.Realtime; // ���� ���� ���� ���̺귯��
using UnityEngine;
using UnityEngine.UI;

public class lobbymanager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // ���� ����

    public Text connectionInfoText; // ��Ʈ��ũ ������ ǥ���� �ؽ�Ʈ
    public Button joinButton; // �� ���� ��ư

    public InputField nick_name_input;
    public string nick_name;

    public string temp;
    // ���� ����� ���ÿ� ������ ���� ���� �õ�
    private void Start()
    {
        // ���ӿ� �ʿ��� ����(���� ����) ����
        PhotonNetwork.GameVersion = gameVersion;
        // ������ ������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();

        // �� ���� ��ư�� ��� ��Ȱ��ȭ
        joinButton.interactable = false;
        connectionInfoText.text = "������ ������ ������...";
    }

    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "�¶��� : ������ ������ �����";
    }

    // ������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        // �� ���� ��ư�� ��Ȱ��ȭ
        joinButton.interactable = false;
        // ���� ���� ǥ��
        connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";

        // ������ �������� ������ �õ�
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        // �ߺ� ���� �õ��� ���� ���� ���� ��ư ��� ��Ȱ��ȭ
        joinButton.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "�뿡 ����...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "�� ���� ����, ���ο� �� ����...";

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    // �뿡 ���� �Ϸ�� ��� �ڵ� ����
    public override void OnJoinedRoom()
    {
        // ���� ���� ǥ��
        connectionInfoText.text = "�� ���� ����";
        // �κ�� ������

        PhotonNetwork.LoadLevel("lobby");
    }


    public void input_nickname()
    {
        nick_name = nick_name_input.text;
        
        PhotonNetwork.LocalPlayer.NickName = nick_name;


        ExitGames.Client.Photon.Hashtable user_custom_value = new ExitGames.Client.Photon.Hashtable();

        /*
        
        S
        //input_nickname();

        user_custom_value["nickname"] = nick_name;
        PhotonNetwork.LocalPlayer.SetCustomProperties(user_custom_value);

        // Ŀ���� �Ӽ��� ������ �Ŀ� ���� ��������
        if (user_custom_value.TryGetValue("nickname", out object nickname_temp))
        {
            temp = (string)nickname_temp;
            Debug.Log("Nickname: " + temp);
        }
        */
    }
}

