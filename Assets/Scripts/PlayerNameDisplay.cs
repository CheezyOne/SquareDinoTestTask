using Mirror;
using TMPro;
using UnityEngine;

public class PlayerNameDisplay : NetworkBehaviour
{
    [SerializeField] private GameObject _nameCanvas;
    [SerializeField] private TMP_Text _nameText;

    private const string DEFAULT_NAME = "Player";

    [SyncVar(hook = nameof(OnNameChanged))]
    private string _playerName;

    public override void OnStartLocalPlayer()
    {
        string savedName = PlayerPrefs.GetString("PlayerName", DEFAULT_NAME);
        if (string.IsNullOrEmpty(savedName))
            savedName = DEFAULT_NAME;

        CmdSetPlayerName(savedName);
    }

    [Command]
    private void CmdSetPlayerName(string name)
    {
        _playerName = name;
    }

    private void OnNameChanged(string oldName, string newName)
    {
        _nameText.text = newName;
    }

    private void Update()
    {
        if (isOwned && Input.GetKeyDown(KeyCode.Space))
        {
            CmdSendChatMessage();
        }

        _nameCanvas.transform.LookAt(Camera.main.transform);
        _nameCanvas.transform.Rotate(0, 180, 0);
    }

    [Command]
    private void CmdSendChatMessage()
    {
        RpcShowChatMessage(_playerName);
    }

    [ClientRpc]
    private void RpcShowChatMessage(string name)
    {
        Debug.Log($"Привет от {name}");
    }
}