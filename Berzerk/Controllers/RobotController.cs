using Berzerk.DTOs;
using Berzerk.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Berzerk.DTOs.CharacterDTO;

namespace Berzerk.Controllers
{
    internal class RobotController
    {
        public bool isAlive;

        public MapDTO.Entity robotEntity;
        public PictureBox robotBody;

        public RobotMindset robotMindset;
        public RobotState robotState;
        public RobotType robotType;

        private Direction direction;
        private List<Direction> listOfDirections = new List<Direction>();
        private Pathfind pathfind;
        private Panel? gamePanel;

        private PlayerController player;
        Random random = new Random();

        public RobotController(MapDTO.Entity entity, RobotMindset robotMindset, RobotState robotState, RobotType robotType, Pathfind pathfind, PlayerController player, Panel gamePanel)
        {
            isAlive = true;

            this.robotEntity = entity;
            this.robotBody = entity.pictureBox;

            direction = Direction.Down;

            this.robotMindset = robotMindset;
            this.robotState = robotState;
            this.robotType = robotType;

            this.pathfind = pathfind;
            this.player = player;
            this.gamePanel = gamePanel;
        }

        public void NextMove(int movementSpeed)
        {
            if (random.Next(0, 100) > 98) // 1% chance for robot to change state
            {
                switch (robotMindset)
                {
                    case RobotMindset.Aggressive:
                        {
                            var number = random.Next(0, 100);

                            if (number > 40)
                                robotState = RobotState.Moving;
                            else
                                robotState = RobotState.Attacking;
                        }
                        break;
                    case RobotMindset.Deffensive:
                        {
                            var number = random.Next(0, 100);

                            if (number > 97)
                                robotState = RobotState.Attacking;
                            else
                                robotState = RobotState.Moving;
                        }
                        break;
                    case RobotMindset.Passive:
                        {
                            var number = random.Next(0, 100);

                            if (number > 90)
                                robotState = RobotState.Moving;
                            else
                                robotState = RobotState.Idle;
                        }
                        break;
                    case RobotMindset.Random:
                        {
                            robotState = (RobotState)random.Next(0, 4);
                        }
                        break;
                }
            }
            switch (robotState)
            {
                case RobotState.Idle:
                    {
                        //if(random.Next(0, 100) > 90)
                            //direction = (Direction)random.Next(0, 8);
                        //MoveRobot(direction, movementSpeed);
                    }
                    break;
                case RobotState.Attacking:
                    {
                        if (player.playerBody != null && robotBody != null)
                        {
                            var pathToPlayer = pathfind.GetPathFromObjectToTarget(new Tuple<int, int>(robotBody.Location.X / 10, robotBody.Location.Y / 10), new Tuple<int, int>(player.playerBody.Location.X / 10, player.playerBody.Location.Y / 10), false);
                            if (pathToPlayer.Count > 0)
                                direction = pathToPlayer.First();
                            else
                                direction = (Direction)random.Next(0, 8);
                        }
                        else
                            direction = (Direction)random.Next(0, 8);

                        MoveRobot(direction, movementSpeed);
                    }
                    break;
                case RobotState.Moving:
                    {
                        if (robotBody != null)
                        {
                            if(listOfDirections.Count == 0)
                            {
                                listOfDirections = pathfind.GetPathFromObjectToTarget(new Tuple<int, int>(robotBody.Location.X / 10, robotBody.Location.Y / 10), new Tuple<int, int>(random.Next(0, 125), random.Next(0, 70)), false);
                            }
                            
                            if(listOfDirections.Count > 0)
                            {
                                direction = listOfDirections.First();
                                listOfDirections.RemoveAt(0);
                            }
                        }
                        else
                            direction = (Direction)random.Next(0, 8);

                        MoveRobot(direction, movementSpeed);
                    }
                    break;
            }
        }

        public void CheckRobotStatus(MapControllers map)
        {
            if(map.IsTouchingMap(this) == CharacterState.Dead)
            {
                isAlive = false;
                robotBody.Dispose();
                RemoveRobot();
            }
        }
        private void RemoveRobot()
        {
            MapDTO.Map.Remove(robotEntity);
        }

        public void UpdateMap()
        {
            robotEntity.pictureBox = robotBody;
            MapDTO.Map[MapDTO.Map.IndexOf(robotEntity)] = robotEntity;
        }

        public void MoveRobot(CharacterDTO.Direction direction, int movementSpeed)
        {
            switch (direction)
            {
                case Direction.Up:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X, robotBody.Location.Y - movementSpeed);
                    break;
                case Direction.Down:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X, robotBody.Location.Y + movementSpeed);
                    break;
                case Direction.Left:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X - movementSpeed, robotBody.Location.Y);
                    break;
                case Direction.Right:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X + movementSpeed, robotBody.Location.Y);
                    break;
                case Direction.UpLeft:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X - movementSpeed, robotBody.Location.Y - movementSpeed);
                    break;
                case Direction.UpRight:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X + movementSpeed, robotBody.Location.Y - movementSpeed);
                    break;
                case Direction.DownLeft:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X - movementSpeed, robotBody.Location.Y + movementSpeed);
                    break;
                case Direction.DownRight:
                    robotBody.Location = new System.Drawing.Point(robotBody.Location.X + movementSpeed, robotBody.Location.Y + movementSpeed);
                    break;
            }

            UpdateMap();
        }

        internal void Shoot(int bulletSpeed)
        {
            var entity = new MapDTO.Entity();
            entity.type = MapDTO.EntityType.EnemyBullet;
            entity.pictureBox = new System.Windows.Forms.PictureBox();
            entity.pictureBox.BackColor = System.Drawing.Color.DarkOrange;
            entity.pictureBox.Location = new System.Drawing.Point(robotBody.Location.X + 15, robotBody.Location.Y + 15);
            entity.pictureBox.Size = new System.Drawing.Size(10, 10);

            var bullet = new BulletController(entity, bulletSpeed, direction);
            MapDTO.Map.Add(entity);
            MapDTO.bullets.Add(bullet);

            gamePanel!.Controls.Add(bullet.bulletBody);
        }
    }
}
