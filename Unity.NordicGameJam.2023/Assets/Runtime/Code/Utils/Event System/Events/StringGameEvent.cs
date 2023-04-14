using System;
using UnityEngine;

namespace Amheklerior.NordicGameJam2023.Utils
{
    [Serializable]
    [CreateAssetMenu(menuName = GameEventUtility.GAME_EVENT_MENU_ROOT + "String event", order = 3)]
    public class StringGameEvent : GameEvent<string> { }
}