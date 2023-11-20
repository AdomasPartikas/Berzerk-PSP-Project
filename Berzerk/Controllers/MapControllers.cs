using Berzerk.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Berzerk.DTOs;
using Berzerk.Utils;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;

namespace Berzerk.Controllers
{
    internal class MapControllers
    {
        public List<List<MapDTO.Entity>> listOfMaps = new List<List<MapDTO.Entity>>();

        public Game? game;
        private Panel? gamePanel;

        public MapControllers(Game? game)
        {
            try
            {
                this.game = game ?? throw new NullReferenceException();
                this.gamePanel = game.Controls.Find("gamePanel", true).FirstOrDefault() as Panel;

                listOfMaps = new MapBuilder().CreateMaps();
            }
            catch (Exception)
            {
                MessageBox.Show("Game panel not found", "Null Reference", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public CharacterDTO.CharacterState IsTouchingMap<T>(T controller)
        {
            if(controller is PlayerController)
            {
                var player = controller as PlayerController;
                var playerBody = player.playerBody;

                foreach (var entity in MapDTO.Map)
                {
                    if (entity.type == MapDTO.EntityType.Wall &&
                        playerBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.Dead;
                    }
                    else if (entity.type == MapDTO.EntityType.Robot &&
                        playerBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.Dead;
                    }
                    else if (entity.type == MapDTO.EntityType.EnemyBullet &&
                        playerBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.Dead;
                    }
                    else if(entity.type == MapDTO.EntityType.Otto &&
                        playerBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.Dead;
                    }
                    else if(entity.type == MapDTO.EntityType.ExitDoor &&
                        playerBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.NextLevel;
                    }
                }
            }
            else if(controller is RobotController)
            {
                var robot = controller as RobotController;
                var robotBody = robot.robotBody;

                foreach (var entity in MapDTO.Map)
                {
                    if (entity.type == MapDTO.EntityType.Wall &&
                        robotBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.Dead;
                    }
                    else if (entity.type == MapDTO.EntityType.PlayerBullet &&
                        robotBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.Dead;
                    }
                }
            }
            else if(controller is BulletController)
            {
                var bullet = controller as BulletController;
                var bulletBody = bullet.bulletBody;

                foreach (var entity in MapDTO.Map)
                {
                    if (entity.type == MapDTO.EntityType.Wall &&
                        bulletBody.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                    {
                        return CharacterDTO.CharacterState.Dead;
                    }
                }
            }

            return CharacterDTO.CharacterState.Alive;
        }

        public void LoadMap()
        {
            foreach (var entity in MapDTO.Map)
            {
                gamePanel!.Controls.Add(entity.pictureBox);
            }
        }
    }
}
