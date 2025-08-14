using Fusion;
using FusionExamples.Utility;
using System.Collections;
using UnityEngine;

namespace Managers
{
    public class LevelManager : NetworkSceneManagerDefault
    {
        public const int LAUNCH_SCENE = 0;
        public const int LOBBY_SCENE = 1;
        public const int GARAGE_SCENE = 4;

        [SerializeField]
        private UIScreen _dummyScreen;
        [SerializeField]
        private UIScreen _lobbyScreen;
        [SerializeField]
        private CanvasFader fader;

        public static LevelManager Instance => Singleton<LevelManager>.Instance;

        public static void LoadMenu()
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(LOBBY_SCENE));
        }

        public static void LoadTrack(int sceneIndex)
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(sceneIndex));
        }

        public static void LoadGarage()
        {
            Instance.Runner.LoadScene(SceneRef.FromIndex(3));
        }

        protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef, NetworkLoadSceneParameters sceneParams)
        {
            Debug.Log($"Loading scene {sceneRef}");

            PreLoadScene(sceneRef.AsIndex);

            yield return base.LoadSceneCoroutine(sceneRef, sceneParams);

            yield return null;

            if (GameManager.CurrentTrack != null && sceneRef.AsIndex > LOBBY_SCENE)
            {
                if (Runner.GameMode == GameMode.Host)
                {
                    foreach (var player in RoomPlayer.Players)
                    {
                        player.GameState = RoomPlayer.EGameState.GameCutscene;
                        GameManager.CurrentTrack.SpawnPlayer(Runner, player);
                    }

                    for (int i = 0; i < RoomPlayer.Players.Count; i++)
                        RoomPlayer.Players[i].Car.CarEntity.CarPartsHolder.SetData(RoomPlayer.Players[i].CarBodyId, RoomPlayer.Players[i].CarWheelID, RoomPlayer.Players[i].CarSpolerID);
                }
            }

            PostLoadScene();
        }

        private void PreLoadScene(int scene)
        {
            if (scene > LOBBY_SCENE)
            {
                Debug.Log("Showing Dummy");
                UIScreen.Focus(_dummyScreen);
            }
            else if (scene == LOBBY_SCENE)
            {
                foreach (RoomPlayer player in RoomPlayer.Players)
                    player.IsReady = false;
                UIScreen.activeScreen.BackTo(_lobbyScreen);
            }
            else
            {
                UIScreen.BackToInitial();
            }
            fader.gameObject.SetActive(true);
            fader.FadeIn();
        }

        private void PostLoadScene() =>
            fader.FadeOut();
    }
}