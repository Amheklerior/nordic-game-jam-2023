using System;
using UnityEngine;

namespace Amheklerior.NordicGameJam2023.Utils
{
    [Serializable]
    [CreateAssetMenu(menuName = GameEventUtility.GAME_EVENT_MENU_ROOT + "GameObject event", order = 10)]
    public class GameObjectGameEvent : GameEvent<GameObject> { }
}