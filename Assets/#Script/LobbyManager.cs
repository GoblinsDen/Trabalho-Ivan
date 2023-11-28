using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerListContent;
    [SerializeField] private GameObject playerListItemPrefab;
    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerListItem(otherPlayer);
    }

    private void AddPlayerListItem(Player player)
    {
        if (playerListItems.ContainsKey(player.ActorNumber))
            return;

        GameObject item = Instantiate(playerListItemPrefab, playerListContent.transform);
        item.GetComponentInChildren<Text>().text = player.NickName; // Assume there is a Text component in children
        playerListItems[player.ActorNumber] = item;
    }

    private void RemovePlayerListItem(Player player)
    {
        Destroy(playerListItems[player.ActorNumber]);
        playerListItems.Remove(player.ActorNumber);
    }

    [PunRPC]
    public void UpdatePlayerReadyState(int playerID, bool playerReady)
    {
        GameObject item;
        if (playerListItems.TryGetValue(playerID, out item))
        {
            // Assuming 'playerListItems' is a Dictionary that maps player IDs to their UI GameObjects.
            Player player = PhotonNetwork.CurrentRoom.GetPlayer(playerID);
            if (player != null)
            {
                item.GetComponentInChildren<Text>().text = playerReady ? player.NickName + " (Ready)" : player.NickName;
            }
            else
            {
                Debug.LogError("Player with ID " + playerID + " not found!");
            }
        }
    }

}
