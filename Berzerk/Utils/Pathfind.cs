using Berzerk.Controllers;
using Berzerk.DTOs;
using Berzerk.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berzerk.Utils
{
    public class Pathfind
    {
        private Tile[,]? map;
        private Tile[,] mapCopy;
        public Game? game;
        private Panel? gamePanel;
        public int pahtfindIterations = 15;

        struct Tile
        {
            public int x;
            public int y;

            public MapDTO.PathfindEntityType type;

            public double g;
            public double h;
            public double f;

            public int parentX;
            public int parentY;
        }
        public Pathfind(Game? game)
        {
            try
            {
                this.game = game ?? throw new NullReferenceException();
                this.gamePanel = game.Controls.Find("gamePanel", true).FirstOrDefault() as Panel;

                ModelNewMap();
            }
            catch (Exception)
            {
                throw new NullReferenceException();
            }
        }
        public List<CharacterDTO.Direction> GetPathFromObjectToTarget(Tuple<int, int> addresser, Tuple<int, int> target, bool ottoMode, int pathfindIterations)
        {
            if (map == null)
            {
                ModelNewMap();
            }

            mapCopy = new Tile[MapDTO.MapWidth, MapDTO.MapHeight];

            if (!ottoMode)
                mapCopy = (Tile[,])map.Clone();
            else
            {
                for (int i = 0; i < MapDTO.MapWidth; i++)
                {
                    for (int j = 0; j < MapDTO.MapHeight; j++)
                    {
                        mapCopy[i, j].type = MapDTO.PathfindEntityType.Empty;
                        mapCopy[i, j].x = i;
                        mapCopy[i, j].y = j;
                    }
                }
            }

            List<Tile> openTiles = new List<Tile>();
            List<Tile> closedTiles = new List<Tile>();

            mapCopy[addresser.Item1, addresser.Item2].g = 0;
            mapCopy[addresser.Item1, addresser.Item2].h = Math.Sqrt(Math.Pow(target.Item1 - addresser.Item1, 2) + Math.Pow(target.Item2 - addresser.Item2, 2));
            mapCopy[addresser.Item1, addresser.Item2].f = mapCopy[addresser.Item1, addresser.Item2].g + mapCopy[addresser.Item1, addresser.Item2].h;
            mapCopy[addresser.Item1, addresser.Item2].type = MapDTO.PathfindEntityType.Adresser;

            openTiles.Add(mapCopy[addresser.Item1, addresser.Item2]);

            mapCopy[target.Item1, target.Item2].type = MapDTO.PathfindEntityType.Target;
            mapCopy[target.Item1, target.Item2].g = Math.Sqrt(Math.Pow(addresser.Item1 - target.Item1, 2) + Math.Pow(addresser.Item2 - target.Item2, 2));
            mapCopy[target.Item1, target.Item2].h = 0;
            mapCopy[target.Item1, target.Item2].f = mapCopy[target.Item1, target.Item2].g + mapCopy[target.Item1, target.Item2].h;

            for(int i = 0; i < pahtfindIterations; i++)
            {
                openTiles.Sort((x, y) => x.f.CompareTo(y.f));

                if(openTiles.Count == 0)
                {
                    break;
                }
                if(openTiles.First().type == MapDTO.PathfindEntityType.Target)
                {
                    break;
                }
                else
                {
                    openTiles.AddRange(OpenNewTiles(openTiles.First(), mapCopy[addresser.Item1, addresser.Item2], mapCopy[target.Item1, target.Item2]));

                    var newTile = openTiles.First();
                    newTile.type = newTile.type == MapDTO.PathfindEntityType.Adresser ? MapDTO.PathfindEntityType.Adresser : MapDTO.PathfindEntityType.ClosedTile;
                    mapCopy[newTile.x, newTile.y] = newTile;


                    closedTiles.Add(newTile);
                    openTiles.Remove(openTiles.First());
                }
            }

            return GetDirections(closedTiles);
        }

        private List<CharacterDTO.Direction> GetDirections(List<Tile> closedTiles)
        {
            var currentPostion = closedTiles.Last();
            var directions = new List<CharacterDTO.Direction>();

            while(true)
            {
                if(currentPostion.type == MapDTO.PathfindEntityType.Adresser)
                {
                    break;
                }
                foreach(var tile in closedTiles)
                {
                    if(tile.x == currentPostion.parentX && tile.y == currentPostion.parentY)
                    {
                        if(tile.x == currentPostion.x && tile.y == currentPostion.y + 1)
                        {
                            directions.Add(CharacterDTO.Direction.Up);
                        }
                        else if (tile.x == currentPostion.x && tile.y == currentPostion.y - 1)
                        {
                            directions.Add(CharacterDTO.Direction.Down);
                        }
                        else if (tile.x == currentPostion.x - 1 && tile.y == currentPostion.y)
                        {
                            directions.Add(CharacterDTO.Direction.Right);
                        }
                        else if (tile.x == currentPostion.x + 1 && tile.y == currentPostion.y)
                        {
                            directions.Add(CharacterDTO.Direction.Left);
                        }
                        else if (tile.x == currentPostion.x - 1 && tile.y == currentPostion.y + 1)
                        {
                            directions.Add(CharacterDTO.Direction.UpRight);
                        }
                        else if (tile.x == currentPostion.x + 1 && tile.y == currentPostion.y + 1)
                        {
                            directions.Add(CharacterDTO.Direction.UpLeft);
                        }
                        else if (tile.x == currentPostion.x - 1 && tile.y == currentPostion.y - 1)
                        {
                            directions.Add(CharacterDTO.Direction.DownRight);
                        }
                        else if (tile.x == currentPostion.x + 1 && tile.y == currentPostion.y - 1)
                        {
                            directions.Add(CharacterDTO.Direction.DownLeft);
                        }

                        currentPostion = tile;
                        break;
                    }
                }   
            }
            return directions;
        }

        private List<Tile> OpenNewTiles(Tile tile, Tile adresser, Tile target)
        {
            var newTiles = new List<Tile>();

            for(int i = tile.x - 1; i <= tile.x + 1; i++)
            {
                for(int j = tile.y - 1; j <= tile.y + 1; j++)
                {
                    if(i < 0 || i >= MapDTO.MapWidth || j < 0 || j >= MapDTO.MapHeight)
                    {
                        continue;
                    }
                    else if (i == tile.x && j == tile.y)
                    {
                        continue;
                    }
                    else if (mapCopy[i,j].type == MapDTO.PathfindEntityType.Wall || mapCopy[i,j].type == MapDTO.PathfindEntityType.OpenTile ||
                                mapCopy[i,j].type == MapDTO.PathfindEntityType.ClosedTile || mapCopy[i,j].type == MapDTO.PathfindEntityType.Adresser)
                    {
                        continue;
                    }

                    mapCopy[i, j].g = Math.Sqrt(Math.Pow(adresser.x - i, 2) + Math.Pow(adresser.y - j, 2));
                    mapCopy[i, j].h = Math.Sqrt(Math.Pow(target.x - i, 2) + Math.Pow(target.y - j, 2));
                    mapCopy[i, j].f = mapCopy[i, j].g + mapCopy[i, j].h;
                    mapCopy[i, j].parentX = tile.x;
                    mapCopy[i, j].parentY = tile.y;
                    mapCopy[i,j].type = mapCopy[i,j].type == MapDTO.PathfindEntityType.Target ? MapDTO.PathfindEntityType.Target : MapDTO.PathfindEntityType.OpenTile;

                    newTiles.Add(mapCopy[i, j]);
                }
            }

            return newTiles;
        }

        public void ModelNewMap()
        {
            map = new Tile[MapDTO.MapWidth, MapDTO.MapHeight];
            for (int i = 0; i < MapDTO.MapWidth; i++)
            {
                for (int j = 0; j < MapDTO.MapHeight; j++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Width = 15;
                    pictureBox.Height = 15;
                    pictureBox.Location = new System.Drawing.Point(i * 10, j * 10);
                    pictureBox.Name = $"testBox";

                    foreach (var entity in MapDTO.Map)
                    {
                        if((entity.type == MapDTO.EntityType.Wall || entity.type == MapDTO.EntityType.ExitDoor
                            || entity.type == MapDTO.EntityType.StartDoor) && pictureBox.Bounds.IntersectsWith(entity.pictureBox.Bounds))
                        {
                            map[i, j].type = MapDTO.PathfindEntityType.Wall;
                            break;
                        }
                        else
                        {
                            map[i, j].type = MapDTO.PathfindEntityType.Empty;
                        }
                    }

                    map[i, j].x = i;
                    map[i, j].y = j;
                }
            }
        }
    }
}
