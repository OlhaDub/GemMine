using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;
using TMPro;
using System.Threading.Tasks;
using System;

public class LeaderBoardManager : MonoBehaviour
{
    string leaderBoardId = "21655";
    string leaderBoardKey = "GemMine";

    public TMP_InputField inputName;
    public TMP_Text score;

    public List<TMP_Text> names;
    public List<TMP_Text> scores;

    

    public IEnumerator ScoreRoutine(int scoreToUpload)
    {
        bool done = false;
        string playerId = inputName.text;

        LootLockerSDKManager.SubmitScore(playerId, scoreToUpload, leaderBoardId, (response) =>
        {
            if (response.success) {
                Debug.Log("SuccessfullyUploded score");
                done = true;
            }
            else
            {
                Debug.Log("Failed" + response.errorData);
                done = true;
            }
        });
        yield return new WaitWhile(()=>done==false);
    }

    public void AddScore()
    {
        int scoreForPlayer = int.Parse(score.text);
        StartCoroutine(ScoreRoutine(scoreForPlayer));
    }

    public IEnumerator FetchTopHihgScores()
    {
        if (names != null)
        {
            bool done = false;
            LootLockerSDKManager.GetScoreList(leaderBoardKey, 10, 0, (response) =>
            {
                if (response.success)
                {
                    LootLockerLeaderboardMember[] members = response.items;

                    for (int i = 0; i < members.Length; i++)
                    {
                        names[i].text = members[i].member_id;
                        scores[i].text = Convert.ToString(members[i].score);
                        Debug.Log(members[i].member_id + " " + Convert.ToString(members[i].score));
                        Debug.Log(names[i].text + " " + scores[i].text);
                    }
                    done = true;
                }
                else
                {
                    Debug.Log("Fail " + response.errorData);
                    done = true;
                }
            });
            yield return new WaitWhile(() => done == false);
        }
    }
}
