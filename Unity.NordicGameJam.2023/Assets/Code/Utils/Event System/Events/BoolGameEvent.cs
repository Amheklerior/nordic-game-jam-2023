using System;
using UnityEngine;

namespace NordicGameJam2023.Utils
{
    [Serializable]
    [CreateAssetMenu(menuName = GameEventUtility.GAME_EVENT_MENU_ROOT + "Bool event", order = 2)]
    public class BoolGameEvent : GameEvent<bool> { }
}