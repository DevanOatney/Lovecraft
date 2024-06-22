using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    public void OnGameOver()
    {
        GameEventSystem.Instance.TriggerEvent(GameEvent.GAME_OVER);
    }
}
