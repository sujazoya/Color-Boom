using UnityEngine;
using System.Collections;

public class CommonKeyListener : MonoBehaviour
{

    void Update()
    {
        if (GameController.Instance == null) return;

   
        // Back Button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (GameController.Instance.currentGameState)
            {
                case GameController.GameState.None:
                    Application.Quit();
                    break;
                case GameController.GameState.OnMainMenu:

                    if (GuiController.Instance.popupDialogOpen == true)
                    {
                        if (GuiController.Instance.cancelPopupDelegate != null)
                        {
                            GuiController.Instance.cancelPopupDelegate.Execute();
                        }
                        return;
                    }

                    GuiController.Instance.showDialogPoup("EXIT", Color.yellow, "ARE YOU SURE YOU WANT TO EXIT ?", delegate () { Application.Quit(); }, null);

                    break;
                case GameController.GameState.OnGameEnter:
                    GameController.Instance.setOnMainMenu();
                    break;
                case GameController.GameState.OnGamePlaying:
                    GameController.Instance.setOnPause();
                    break;
                case GameController.GameState.OnPaused:
                    GameController.Instance.setOnResume();
                    break;
                case GameController.GameState.OnFailed:
                    GameController.Instance.setOnMainMenu();
                    break;
                case GameController.GameState.OnSucceed:
                    GameController.Instance.setOnMainMenu();
                    break;
            }
        }



        // Enter Button
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (GameController.Instance.currentGameState)
            {
                case GameController.GameState.None:

                    break;
                case GameController.GameState.OnMainMenu:
                    GameController.Instance.btnMainMenuPlayOnClick();
                    GameObject.Find("Circles").GetComponent<TweenScale>().PlayForward();
                    break;
                case GameController.GameState.OnGameEnter:
                    break;
                case GameController.GameState.OnGamePlaying:
                    GameController.Instance.setOnPause();
                    break;
                case GameController.GameState.OnPaused:
                    GameController.Instance.setOnResume();
                    break;
                case GameController.GameState.OnFailed:
                    GameController.Instance.btnFailedMenuReplayOnClick();
                    break;
                case GameController.GameState.OnSucceed:
                    GameController.Instance.btnSuccesMenuNextLevelOnClick();
                    break;
            }
        }

    }

}
