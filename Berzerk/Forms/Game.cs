using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Berzerk.Controllers;
using Berzerk.Utils;
using Berzerk.DTOs;
using System.Diagnostics;
using Berzerk.Abstraction;
using Microsoft.VisualBasic.Devices;
using System.Windows.Input;

namespace Berzerk.Forms
{

    public partial class Game : Form
    {
        private PlayerController player;
        private OttoController otto;
        private MapControllers map;
        private Pathfind pathfind;
        private bool isGameRunning = false;

        private int bulletSpeed = 5;
        private int movementSpeed = 1;
        private int gameSpeed = 10;
        private bool isOttoAlive = false;

        private int roundCount = 0;
        private int roundRecord = 0;

        Random rand = new Random();

        public Game()
        {
            InitializeComponent();
            MapDTO.MapWidth = gamePanel.Width / 10;
            MapDTO.MapHeight = gamePanel.Height / 10;

            StartNewRound();
        }

        private void StartNewRound()
        {
            MapDTO.Map.ForEach(entity => entity.pictureBox.Dispose());
            MapDTO.Map.Clear();
            gamePanel.Controls.Clear();
            gamePanel.Refresh();
            MapDTO.robots.Clear();
            MapDTO.bullets.Clear();
            isOttoAlive = false;

            map = new MapControllers(this);

            UpdateRounds();

            GC.Collect();

            tickTimer.Interval = gameSpeed;

            while(MapDTO.Map.Count == 0)
            {
                MapDTO.Map.AddRange(map.listOfMaps[rand.Next(0, map.listOfMaps.Count)].ToList());
                map.LoadMap();
            }

            AbstractController playerController = MapDTO.playerFactory.CreateController(gamePanel);
            player = (PlayerController)playerController;

            pathfind = new Pathfind(this);
            CreateRobots();

            isGameRunning = true;
        }

        private void UpdateRounds()
        {
            roundCount++;
            
            if(roundCount > roundRecord)
                roundRecord = roundCount;

            this.Text = $"Berzerk\t\tRound: {roundCount} / Record: {roundRecord}";
        }

        private void CreateRobots()
        {
            foreach (var entity in MapDTO.Map)
            {
                if (entity.type == MapDTO.EntityType.Robot)
                {
                    var robotMindset = (CharacterDTO.RobotMindset)rand.Next(0, 3);
                    var robotState = (CharacterDTO.RobotState)rand.Next(0,2);
                    var robotType = (CharacterDTO.RobotType)rand.Next(0, 2);


                    AbstractController robotController = MapDTO.robotFactory.CreateController(entity, robotMindset, robotState, robotType, pathfind, player, gamePanel);

                    MapDTO.robots.Add((RobotController)robotController);
                }
            }
        }

        private void RobotTurn()
        {
            foreach (var robot in MapDTO.robots)
            {
                robot.NextMove(movementSpeed);
                robot.CheckEntityStatus(map, robot.robotEntity, robot.robotBody);

                if(robot.robotState == CharacterDTO.RobotState.Attacking && rand.Next(0,100) >= 93)
                {
                    robot.Shoot(bulletSpeed, MapDTO.EntityType.EnemyBullet, robot.direction, robot.robotBody, gamePanel);
                }
            }
            MapDTO.robots = MapDTO.robots.Where(robot => robot.isAlive == true).ToList();
            if(MapDTO.robots.Any(robot => robot.isAlive == false))
            {
                RemoveControl(0);
            }
        }

        private void RemoveControl(int index)
        {
            switch(index)
            {
                case 0:
                    {
                        foreach (var robot in MapDTO.robots)
                        {
                            if (robot.isAlive == false)
                            {
                                gamePanel.Controls.Remove(robot.robotBody);
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        foreach (var bullet in MapDTO.bullets)
                        {
                            if (bullet.isAlive == false)
                            {
                                gamePanel.Controls.Remove(bullet.bulletBody);
                            }
                        }
                    }
                    break;
            }
        }

        private void UpdateBullets()
        {

            foreach (var bullet in MapDTO.bullets)
            {
                bullet.Move(bullet.bulletEntity, bullet.direction, bullet.bulletBody, bullet.bulletSpeed);
                bullet.CheckEntityStatus(map, bullet.bulletEntity, bullet.bulletBody);
            }
            MapDTO.bullets = MapDTO.bullets.Where(bullet => bullet.isAlive == true).ToList();
            if (MapDTO.bullets.Any(bullet => bullet.isAlive == false))
            {
                RemoveControl(1);
            }
        }

        private void PlayerTurn()
        {
            player.GetRelativeCursorPosition();
            player.Move(player.playerEntity, player.GetRelativeCursorPosition(), player.playerBody, movementSpeed);
            player.CheckEntityStatus(map, player.playerEntity, player.playerBody);
        }

        private void PlayerKeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                player.Shoot(bulletSpeed, MapDTO.EntityType.PlayerBullet, player.GetRelativeCursorPosition(), player.playerBody, gamePanel);
            }
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            if (!isGameRunning)
                return;

            if(player.isAlive == false)
            {
                isGameRunning = false;
                MessageBox.Show($"You died! You survived {roundCount} rounds!");
                roundCount = 0;
                StartNewRound();
                return;
            }
            else if(player.playerWon == true)
            {
                isGameRunning = false;
                StartNewRound();
                return;
            }
            if(MapDTO.robots.Count == 0 && isGameRunning)
            {
                if(isOttoAlive == false)
                {
                    Debug.WriteLine("SpawningOtto");
                    otto = new OttoController(pathfind, gamePanel, player);
                    isOttoAlive = true;
                }
                else
                {
                    otto.NextMove(movementSpeed);
                }
                
            }
            else
            {
                RobotTurn();
            }

            if (isGameRunning)
            {
                PlayerTurn();
                UpdateBullets();
            }
        }
    }
}
