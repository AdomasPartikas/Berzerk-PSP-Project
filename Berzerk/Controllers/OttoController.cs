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
    public class OttoController : AbstractController
    {
        private bool _isAlive;
        public override bool isAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }

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
            if (player.isAlive)
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

            Move(ottoEntity ,direction, ottoBody, movementSpeed);
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
