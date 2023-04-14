using System;
using UnityEngine;

namespace Amheklerior.NordicGameJam2023.Utils
{
    [Serializable]
    [CreateAssetMenu(menuName = GameEventUtility.GAME_EVENT_MENU_ROOT + "Int event", order = 4)]
    public class GameEventInt : GameEvent<int> { }
}