using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.DTOs
{
    internal class CharacterDTO
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            UpLeft,
            UpRight,
            DownLeft,
            DownRight
        };
        public enum RobotMindset
        {
            Deffensive,
            Aggressive,
            Random,
            Passive
        };
        public enum RobotState
        {
            Idle,
            Attacking,
            Moving
        };
        public enum CharacterState
        {
            Alive,
            Dead,
            NextLevel
        };
        public enum RobotType
        {
            Normal,
            Explosive
        };
    }
}
