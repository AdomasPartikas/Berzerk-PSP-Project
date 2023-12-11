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
    public class BulletController : AbstractController
    {
        private bool _isAlive;
        public override bool isAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        public MapDTO.Entity bulletEntity;
        public PictureBox bulletBody;

        public int bulletSpeed;
        public Direction direction;

        public BulletController(MapDTO.Entity entity, int bulletSpeed, Direction direction)
        {
            isAlive = true;

            this.bulletEntity = entity;
            this.bulletBody = entity.pictureBox;

            this.bulletSpeed = bulletSpeed;
            this.direction = direction;
        }
    }
}
