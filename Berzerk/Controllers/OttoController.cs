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
    internal class OttoController
    {
        public bool isAlive;

        public MapDTO.Entity ottoEntity;
        public PictureBox ottoBody;

        private Direction direction;

        private Pathfind pathfind;
        private Panel? gamePanel;

        private PlayerController player;
        private Random random = new Random();

        public OttoController(Pathfind pathfind, Panel? gamePanel, PlayerController player)
        {
            isAlive = true;

            this.pathfind = pathfind;
            this.gamePanel = gamePanel;
            this.player = player;

            GenerateOtto();
        }

        public void NextMove(int movementSpeed)
        {
            if (player.isPlayerAlive)
            {
                var playerCoordinates = new Tuple<int, int>(player.playerBody.Location.X / 10, player.playerBody.Location.Y / 10);
                var ottoCoordinates = new Tuple<int, int>(ottoBody.Location.X / 10, ottoBody.Location.Y / 10);

                var path = pathfind.GetPathFromObjectToTarget(ottoCoordinates, playerCoordinates, true, 15);

                if (path.Count > 0)
                {
                    direction = path.First();
                }
                else
                {
                    direction = (CharacterDTO.Direction)random.Next(0, 8);
                }
            }
            else
            {
                direction = (CharacterDTO.Direction)random.Next(0, 8);
            }

            MoveOtto(direction, movementSpeed);
        }

        public void MoveOtto(CharacterDTO.Direction direction, int movementSpeed)
        {
            switch (direction)
            {
                case Direction.Up:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X, ottoBody.Location.Y - movementSpeed);
                    break;
                case Direction.Down:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X, ottoBody.Location.Y + movementSpeed);
                    break;
                case Direction.Left:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X - movementSpeed, ottoBody.Location.Y);
                    break;
                case Direction.Right:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X + movementSpeed, ottoBody.Location.Y);
                    break;
                case Direction.UpLeft:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X - movementSpeed, ottoBody.Location.Y - movementSpeed);
                    break;
                case Direction.UpRight:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X + movementSpeed, ottoBody.Location.Y - movementSpeed);
                    break;
                case Direction.DownLeft:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X - movementSpeed, ottoBody.Location.Y + movementSpeed);
                    break;
                case Direction.DownRight:
                    ottoBody.Location = new System.Drawing.Point(ottoBody.Location.X + movementSpeed, ottoBody.Location.Y + movementSpeed);
                    break;
            }

            UpdateMap();
        }

        private void UpdateMap()
        {
            ottoEntity.pictureBox = ottoBody;
            MapDTO.Map[MapDTO.Map.IndexOf(ottoEntity)] = ottoEntity;
        }

        private void GenerateOtto()
        {
            var startingDoor = MapDTO.Map.Select(x => x).Where(x => x.type == MapDTO.EntityType.StartDoor).FirstOrDefault();
            var spawnPoint = new System.Drawing.Point(startingDoor.pictureBox.Location.X + 50, startingDoor.pictureBox.Location.Y);

            ottoEntity = new MapDTO.Entity();
            ottoEntity.type = MapDTO.EntityType.Otto;
            ottoEntity.pictureBox = new System.Windows.Forms.PictureBox();
            ottoEntity.pictureBox.BackColor = System.Drawing.Color.White;
            ottoEntity.pictureBox.Location = spawnPoint;
            ottoEntity.pictureBox.Size = new System.Drawing.Size(30, 30);

            ottoBody = ottoEntity.pictureBox;

            MapDTO.Map.Add(ottoEntity);
            gamePanel!.Controls.Add(ottoBody);
        }
    }
}
