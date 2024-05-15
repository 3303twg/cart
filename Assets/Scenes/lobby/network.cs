using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class network : MonoBehaviourPunCallbacks
{
    // Awake()�� ������ ���۵Ǳ� ���� �����. ��, ���� ���� ���� �ȴ�.
    private void Awake()
    {
        // ������ Ŭ���̾�Ʈ�� PhotonNetwork.LoadLevel()�� ȣ���� �� �ֵ��� �ϰ�,
        // ���� �뿡 �ִ� ��� Ŭ���̾�Ʈ�� ������ ����ȭ�ϰ� ��
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    /// <summary>
    /// ���� ���μ����� ����.
    /// �̹� ������ �Ǿ��ִٸ�, ������ ������
    /// ������ ���� �ʾҴٸ� �ٽ� ����
    /// </summary>
    public void Connect()
    {
        // ���� �Ǿ������� üũ�ؼ�, �뿡 �������� �翬���� �õ����� ����
        if (PhotonNetwork.IsConnected)
        {
            // ���� �뿡 ����.
            // ���ӿ� �����ϸ� OnJoinRandomFailed()�� ����Ǿ� ���� �˸�.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // ���� ���ῡ �����ϸ� ������ ���� �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    // Ŭ���̾�Ʈ�� �����Ϳ� ����Ǹ� ȣ���
    public override void OnConnectedToMaster()
    {
        Debug.Log("Ŭ���̾�Ʈ�� �����Ϳ� �����");
        // �����Ϳ� ����Ǹ� �濡 �������� ����
        PhotonNetwork.JoinRandomRoom();
    }

    // Ŭ���̾�Ʈ�� � ������ε� ������ �������� ȣ���
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("�������� ������ ������. ���� : {0}", cause);
    }

}
