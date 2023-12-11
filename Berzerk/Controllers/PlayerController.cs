using Berzerk.DTOs;
using Berzerk.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Berzerk.DTOs.CharacterDTO;

namespace Berzerk.Controllers
{
    public class PlayerController : AbstractController
    {
        private bool _isAlive;
        public override bool isAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        public bool playerWon;

        public MapDTO.Entity playerEntity;
        public PictureBox playerBody;

        private Panel? gamePanel;

        public PlayerController(Panel gamePanel)
        {
            this.isAlive = true;

            GetPlayer();
            this.gamePanel = gamePanel;
        }

        public Direction GetRelativeCursorPosition()
        {
            var cursorPosition = System.Windows.Forms.Cursor.Position;
            var relativeCursorPositionCoordinates = playerBody.PointToClient(cursorPosition);

            return ConvertCursorCoordinatesToDirection(relativeCursorPositionCoordinates);
        }

        private Direction ConvertCursorCoordinatesToDirection(Point relativeCursorCoordinates)
        {
            double angle = Math.Atan2(relativeCursorCoordinates.Y, relativeCursorCoordinates.X) * 180 / Math.PI;

            if (angle < 0)
            {
                angle += 360;
            }

            if (angle >= 22.5 && angle < 67.5)
            {
                return Direction.DownRight;
            }
            else if (angle >= 67.5 && angle < 112.5)
            {
                return Direction.Down;
            }
            else if (angle >= 112.5 && angle < 157.5)
            {
                return Direction.DownLeft;
            }
            else if (angle >= 157.5 && angle < 202.5)
            {
                return Direction.Left;
            }
            else if (angle >= 202.5 && angle < 247.5)
            {
                return Direction.UpLeft;
            }
            else if (angle >= 247.5 && angle < 292.5)
            {
                return Direction.Up;
            }
            else if (angle >= 292.5 && angle < 337.5)
            {
                return Direction.UpRight;
            }
            else
            {
                return Direction.Right;
            }
        }

        public void GetPlayer()
        {
            foreach (var entity in MapDTO.Map)
            {
                if (entity.type == MapDTO.EntityType.Player)
                {
                    playerEntity = entity;
                }
            }

            playerBody = playerEntity.pictureBox;
        }

    }
}
