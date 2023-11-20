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

            GC.Collect();

            tickTimer.Interval = gameSpeed;

            while(MapDTO.Map.Count == 0)
            {
                MapDTO.Map.AddRange(map.listOfMaps[rand.Next(0, map.listOfMaps.Count)].ToList());
                map.LoadMap();
            }

            player = new PlayerController(gamePanel);
            pathfind = new Pathfind(this);
            CreateRobots();

            isGameRunning = true;
        }

        private void CreateRobots()
        {
            foreach (var entity in MapDTO.Map)
            {
                if (entity.type == MapDTO.EntityType.Robot)
                {
                    var robotMindset = (CharacterDTO.RobotMindset)rand.Next(0, 4);
                    var robotState = (CharacterDTO.RobotState)rand.Next(0,3);
                    var robotType = (CharacterDTO.RobotType)rand.Next(0, 2);

                    MapDTO.robots.Add(new RobotController(entity, robotMindset, robotState, robotType, pathfind, player, gamePanel));
                }
            }
        }

        private void RobotTurn()
        {
            foreach (var robot in MapDTO.robots)
            {
                robot.NextMove(movementSpeed);
                robot.CheckRobotStatus(map);

                if(robot.robotState == CharacterDTO.RobotState.Attacking && rand.Next(0,100) >= 93)
                {
                    robot.Shoot(bulletSpeed);
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
                bullet.MoveBullet();
                bullet.CheckBulletStatus(map);
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
            player.MovePlayer(player.relativeCursorPosition, movementSpeed);
            player.CheckPlayerStatus(map);
        }

        private void PlayerKeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                player.Shoot(bulletSpeed);
            }
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            if (!isGameRunning)
                return;

            if(player.isPlayerAlive == false)
            {
                isGameRunning = false;
                MessageBox.Show("You died!");
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
