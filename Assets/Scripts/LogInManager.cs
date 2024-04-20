using UnityEngine.UI;
using LootLocker.Requests;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class LogInManager : MonoBehaviour
{
    public LeaderBoardManager boardManager;


    private void Start()
    {
        StartCoroutine(LoginRoutine());
    }

    public void ShowBoard()
    {
        StartCoroutine(boardManager.FetchTopHihgScores());
    }

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player was log in");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("Session isnt started");
                done = true;
            }
        });
        yield return new WaitWhile(() => done = false);

    }
}
