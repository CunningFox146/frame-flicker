using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Random = UnityEngine.Random;

namespace CunningFox146.Animation
{
    public class Test : MonoBehaviour
    {
        private async void Awake()
        {
            await UnityServices.InitializeAsync().AsUniTask();
            if (!AuthenticationService.Instance.SessionTokenExists) 
            {
                return;
            }

            // Sign in Anonymously
            // This call will sign in the cached player.
            try
            {
                var signInOptions = new SignInOptions()
                {
                    CreateAccount = true
                };

                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync($"test{Random.Range(0, 1000)}", "Cool!Uniy0006");
                Debug.Log("Sign in anonymously succeeded!");
        
                // Shows how to get the playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
                // await AuthenticationService.Instance.UpdatePlayerNameAsync("dookie").AsUniTask();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            // Debug.Log($"PlayerName: {await AuthenticationService.Instance.GetPlayerNameAsync().AsUniTask()}");
            var score = Random.Range(0, 100);
            var scoresResponse = await  LeaderboardsService.Instance.GetScoresAsync("Sppedrunners");
            var entry = await LeaderboardsService.Instance.AddPlayerScoreAsync("Sppedrunners", Random.Range(0, 100));
            Debug.Log(entry.Rank);
            Debug.Log($"{score} {entry.Score}");

            foreach (var info in (await LeaderboardsService.Instance.GetScoresAsync("Sppedrunners")).Results)
            {
                Debug.Log($"{info.PlayerName} {info.Rank} {info.Score} {info.UpdatedTime}");
            }
        }
    }
}