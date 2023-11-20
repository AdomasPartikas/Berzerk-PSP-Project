using Berzerk.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Berzerk.DTOs.CharacterDTO;

namespace Berzerk.Controllers
{
    internal class PlayerController
    {
        public bool isPlayerAlive;
        public bool playerWon;

        public MapDTO.Entity playerEntity;
        public PictureBox playerBody;

        private Panel? gamePanel;

        public Direction relativeCursorPosition; 

        public PlayerController(Panel gamePanel)
        {
            this.isPlayerAlive = true;

            GetPlayer();
            this.gamePanel = gamePanel;
        }

        public void UpdateMap()
        {
            playerEntity.pictureBox = playerBody;
            MapDTO.Map[MapDTO.Map.IndexOf(playerEntity)] = playerEntity;
        }

        public void GetRelativeCursorPosition()
        {
            var cursorPosition = System.Windows.Forms.Cursor.Position;
            var relativeCursorPositionCoordinates = playerBody.PointToClient(cursorPosition);

            relativeCursorPosition = ConvertCursorCoordinatesToDirection(relativeCursorPositionCoordinates);
        }

        public void Shoot(int bulletSpeed)
        {
            var entity = new MapDTO.Entity();
            entity.type = MapDTO.EntityType.PlayerBullet;
            entity.pictureBox = new System.Windows.Forms.PictureBox();
            entity.pictureBox.BackColor = System.Drawing.Color.DarkCyan;
            entity.pictureBox.Location = new System.Drawing.Point(playerBody.Location.X + 15, playerBody.Location.Y + 15);
            entity.pictureBox.Size = new System.Drawing.Size(10, 10);

            var bullet = new BulletController(entity, bulletSpeed, relativeCursorPosition);
            MapDTO.Map.Add(entity);
            MapDTO.bullets.Add(bullet);

            gamePanel!.Controls.Add(bullet.bulletBody);
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

        public void CheckPlayerStatus(MapControllers map)
        {
            var status = map.IsTouchingMap(this);
            if(status == CharacterDTO.CharacterState.Dead)
            {
                isPlayerAlive = false;
            }
            else if(status == CharacterDTO.CharacterState.NextLevel)
            {
                playerWon = true;
            }
        }

        public void MovePlayer(Direction direction, int movementSpeed)
        {
            switch (direction)
            {
                case Direction.Up:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X, playerBody.Location.Y - movementSpeed);
                    break;
                case Direction.Down:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X, playerBody.Location.Y + movementSpeed);
                    break;
                case Direction.Left:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X - movementSpeed, playerBody.Location.Y);
                    break;
                case Direction.Right:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X + movementSpeed, playerBody.Location.Y);
                    break;
                case Direction.UpLeft:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X - movementSpeed, playerBody.Location.Y - movementSpeed);
                    break;
                case Direction.UpRight:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X + movementSpeed, playerBody.Location.Y - movementSpeed);
                    break;
                case Direction.DownLeft:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X - movementSpeed, playerBody.Location.Y + movementSpeed);
                    break;
                case Direction.DownRight:
                    playerBody.Location = new System.Drawing.Point(playerBody.Location.X + movementSpeed, playerBody.Location.Y + movementSpeed);
                    break;
            }

            UpdateMap();
        }

    }
}
