using Berzerk.Abstraction;
using Berzerk.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.DTOs
{
    public class MapDTO
    {
        public static int MapWidth { get; set;}
        public static int MapHeight { get; set; }

        public static List<Entity> Map = new List<Entity>();

        public static List<RobotController> robots = new List<RobotController>();

        public static List<BulletController> bullets = new List<BulletController>();

        public static ControllerFactory playerFactory = new PlayerControllerFactory();
        public static ControllerFactory robotFactory = new RobotControllerFactory();
        public static ControllerFactory ottoFactory = new OttoControllerFactory();
        public static ControllerFactory bulletFactory = new BulletControllerFactory();

        public enum EntityType
        {
            Wall,
            Player,
            Robot,
            EnemyBullet,
            PlayerBullet,
            StartDoor,
            ExitDoor,
            Empty,
            Explosion,
            Otto
        };

        public enum PathfindEntityType
        {
            Wall,
            Adresser,
            Target,
            Empty,
            OpenTile,
            ClosedTile
        };

        public static Dictionary<EntityType, Color> EntityColorDictionary = new Dictionary<EntityType, Color>()
        {
            {EntityType.Wall, Color.FromArgb(255, 0, 0, 255)},
            {EntityType.Player, Color.FromArgb(255, 0, 255, 0)},
            {EntityType.Robot, Color.FromArgb(255, 255, 0, 0)},
            {EntityType.StartDoor, Color.FromArgb(255, 0, 255, 255)},
            {EntityType.ExitDoor, Color.FromArgb(255, 255, 255, 0)},
            {EntityType.Empty, Color.FromArgb(255, 0, 0, 0)},
        };

        public struct Entity
        {
            public Tuple<int, int> upperLeftCornerCoordinates; //x,y
            public Tuple<int, int> lowerRightCornerCoordinate; //x,y

            public PictureBox pictureBox;

            public EntityType type;
        };
    }
}
