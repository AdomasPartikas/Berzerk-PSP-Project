using Berzerk.Controllers;
using Berzerk.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using static Berzerk.DTOs.CharacterDTO;

namespace Berzerk.Abstraction
{
    public abstract class AbstractController
    {
        public abstract bool isAlive { get; set; }
        public void UpdateMap(PictureBox body, MapDTO.Entity entity) 
        {
            entity.pictureBox = body;
            MapDTO.Map[MapDTO.Map.IndexOf(entity)] = entity;
        }
        public void CheckEntityStatus(MapControllers map, MapDTO.Entity entity, PictureBox body)
        {
            if (entity.type == MapDTO.EntityType.Player)
            {
                var status = map.IsTouchingMap(this);
                if (status == CharacterDTO.CharacterState.Dead)
                {
                    isAlive = false;
                }
                else if (status == CharacterDTO.CharacterState.NextLevel)
                {
                    (this as PlayerController)!.playerWon = true;
                }
            }
            else
            {
                if (map.IsTouchingMap(this) == CharacterDTO.CharacterState.Dead)
                {
                    isAlive = false;
                    body.Dispose();
                    MapDTO.Map.Remove(entity);
                }
            }
        }
        public void Shoot(int bulletSpeed, MapDTO.EntityType entityType, CharacterDTO.Direction direction, PictureBox body, Panel gamePanel)
        {
            var entity = new MapDTO.Entity();
            entity.type = entityType;
            entity.pictureBox = new System.Windows.Forms.PictureBox();

            if(entity.type == MapDTO.EntityType.EnemyBullet)
                entity.pictureBox.BackColor = System.Drawing.Color.DarkOrange;
            else
                entity.pictureBox.BackColor = System.Drawing.Color.DarkCyan;

            entity.pictureBox.Location = new System.Drawing.Point(body.Location.X + 15, body.Location.Y + 15);
            entity.pictureBox.Size = new System.Drawing.Size(10, 10);

            AbstractController bulletController = MapDTO.bulletFactory.CreateController(entity, bulletSpeed, direction);
            var bullet = (BulletController)bulletController;
            MapDTO.Map.Add(entity);
            MapDTO.bullets.Add(bullet);

            gamePanel!.Controls.Add(bullet.bulletBody);
        }
        public void Move(MapDTO.Entity entity ,CharacterDTO.Direction direction, PictureBox body, int movementSpeed)
        {
            switch (direction)
            {
                case Direction.Up:
                    body.Location = new System.Drawing.Point(body.Location.X, body.Location.Y - movementSpeed);
                    break;
                case Direction.Down:
                    body.Location = new System.Drawing.Point(body.Location.X, body.Location.Y + movementSpeed);
                    break;
                case Direction.Left:
                    body.Location = new System.Drawing.Point(body.Location.X - movementSpeed, body.Location.Y);
                    break;
                case Direction.Right:
                    body.Location = new System.Drawing.Point(body.Location.X + movementSpeed, body.Location.Y);
                    break;
                case Direction.UpLeft:
                    body.Location = new System.Drawing.Point(body.Location.X - movementSpeed, body.Location.Y - movementSpeed);
                    break;
                case Direction.UpRight:
                    body.Location = new System.Drawing.Point(body.Location.X + movementSpeed, body.Location.Y - movementSpeed);
                    break;
                case Direction.DownLeft:
                    body.Location = new System.Drawing.Point(body.Location.X - movementSpeed, body.Location.Y + movementSpeed);
                    break;
                case Direction.DownRight:
                    body.Location = new System.Drawing.Point(body.Location.X + movementSpeed, body.Location.Y + movementSpeed);
                    break;
            }

            UpdateMap(body, entity);
        }
    }
}
