using Berzerk.DTOs;
using Berzerk.Abstraction;
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
    public class RobotController : AbstractController
    {
        private bool _isAlive;
        public override bool isAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

        public MapDTO.Entity robotEntity;
        public PictureBox robotBody;

        public RobotMindset robotMindset;
        public RobotState robotState;
        public RobotType robotType;

        public Direction direction;
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

                            if (number > 90)
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
                            robotState = RobotState.Moving;
                        }
                        break;
                }
            }
            switch (robotState)
            {
                case RobotState.Attacking:
                    {
                        var pathToPlayer = pathfind.GetPathFromObjectToTarget(new Tuple<int, int>(robotBody.Location.X / 10, robotBody.Location.Y / 10), new Tuple<int, int>(player.playerBody.Location.X / 10, player.playerBody.Location.Y / 10), false, 15);
                        if (pathToPlayer.Count > 0)
                        {
                            listOfDirections = pathToPlayer;
                            direction = listOfDirections.First();
                            listOfDirections.RemoveAt(0);
                        }
                        else
                        {
                            if (listOfDirections.Count == 0)
                            {
                                listOfDirections = pathfind.GetPathFromObjectToTarget(new Tuple<int, int>(robotBody.Location.X / 10, robotBody.Location.Y / 10), new Tuple<int, int>(random.Next(0, 125), random.Next(0, 70)), false, 30);
                            }

                            if (listOfDirections.Count > 0)
                            {
                                direction = listOfDirections.First();
                                listOfDirections.RemoveAt(0);
                            }
                        }

                        Move(robotEntity, direction, robotBody, movementSpeed);
                    }
                    break;
                case RobotState.Moving:
                    {
                        if (robotBody != null)
                        {
                            if(listOfDirections.Count == 0)
                            {
                                listOfDirections = pathfind.GetPathFromObjectToTarget(new Tuple<int, int>(robotBody.Location.X / 10, robotBody.Location.Y / 10), new Tuple<int, int>(random.Next(0, 125), random.Next(0, 70)), false, 100);
                            }
                            
                            if(listOfDirections.Count > 0)
                            {
                                direction = listOfDirections.First();
                                listOfDirections.RemoveAt(0);
                            }
                        }
                        else
                            direction = (Direction)random.Next(0, 8);

                        Move(robotEntity, direction, robotBody, movementSpeed);
                    }
                    break;
            }
        }
    }
}
