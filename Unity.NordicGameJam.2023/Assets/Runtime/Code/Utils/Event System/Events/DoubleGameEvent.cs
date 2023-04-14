using System;
using UnityEngine;

namespace Amheklerior.NordicGameJam2023.Utils
{
    [Serializable]
    [CreateAssetMenu(menuName = GameEventUtility.GAME_EVENT_MENU_ROOT + "Double event", order = 6)]
    public class DoubleGameEvent : GameEvent<double> { }
}