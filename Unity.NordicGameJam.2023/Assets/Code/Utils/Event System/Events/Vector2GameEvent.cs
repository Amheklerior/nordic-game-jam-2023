using System;
using UnityEngine;

namespace NordicGameJam2023.Utils
{
    [Serializable]
    [CreateAssetMenu(menuName = GameEventUtility.GAME_EVENT_MENU_ROOT + "Float 2D vector event", order = 8)]
    public class Vector2GameEvent : GameEvent<Vector2> { }
}