using Berzerk.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Berzerk.DTOs.CharacterDTO;

namespace Berzerk.Controllers
{
    internal class BulletController
    {
        public bool isAlive;

        public MapDTO.Entity bulletEntity;
        public PictureBox bulletBody;

        private int bulletSpeed;
        private Direction direction;

        public BulletController(MapDTO.Entity entity, int bulletSpeed, Direction direction)
        {
            isAlive = true;

            this.bulletEntity = entity;
            this.bulletBody = entity.pictureBox;

            this.bulletSpeed = bulletSpeed;
            this.direction = direction;
        }

        private void UpdateMap()
        {
            bulletEntity.pictureBox = bulletBody;
            MapDTO.Map[MapDTO.Map.IndexOf(bulletEntity)] = bulletEntity;
        }

        public void CheckBulletStatus(MapControllers map)
        {
            if(map.IsTouchingMap(this) == CharacterDTO.CharacterState.Dead)
            {
                isAlive = false;
                bulletBody.Dispose();
                RemoveBullet();
            }
        }

        private void RemoveBullet()
        {
            MapDTO.Map.Remove(bulletEntity);
        }

        public void MoveBullet()
        {
            switch (direction)
            {
                case Direction.Up:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X, bulletBody.Location.Y - bulletSpeed);
                    break;
                case Direction.Down:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X, bulletBody.Location.Y + bulletSpeed);
                    break;
                case Direction.Left:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X - bulletSpeed, bulletBody.Location.Y);
                    break;
                case Direction.Right:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X + bulletSpeed, bulletBody.Location.Y);
                    break;
                case Direction.UpLeft:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X - bulletSpeed, bulletBody.Location.Y - bulletSpeed);
                    break;
                case Direction.UpRight:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X + bulletSpeed, bulletBody.Location.Y - bulletSpeed);
                    break;
                case Direction.DownLeft:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X - bulletSpeed, bulletBody.Location.Y + bulletSpeed);
                    break;
                case Direction.DownRight:
                    bulletBody.Location = new System.Drawing.Point(bulletBody.Location.X + bulletSpeed, bulletBody.Location.Y + bulletSpeed);
                    break;
            }
            UpdateMap();
        }
    }
}
