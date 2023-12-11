using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Berzerk.DTOs;

namespace Berzerk.Utils
{
    public class MapBuilder
    {
        private bool[,] takenSpaces;
        private string[] listOfMapPaths;
        private string filePath;

        public MapBuilder(string directory)
        {
            if(!Path.Exists(directory))
                throw new FileNotFoundException();
            else
                listOfMapPaths = Directory.GetFiles(directory, "map*.png");
        }

        public List<List<MapDTO.Entity>> CreateMaps()
        {
            var listOfMaps = new List<List<MapDTO.Entity>>();
            foreach (var path in listOfMapPaths)
            {
                var map = new Bitmap(path);
                var mapName = path.Split('\\').Last();
                takenSpaces = new bool[MapDTO.MapWidth, MapDTO.MapHeight];

                if (map.Width != MapDTO.MapWidth || map.Height != MapDTO.MapHeight)
                {
                    var message = $"Map size of map: [{mapName}] does not match the game size";

                    throw new FileFormatException(message);
                }
                else
                {
                    var entitiesInMap = new List<MapDTO.Entity>();
                    var pixel = new Color();

                    for (int x = 0; x < map.Width; x++)
                    {
                        for (int y = 0; y < map.Height; y++)
                        {
                            if (takenSpaces[x, y])
                            {
                                continue;
                            }

                            pixel = map.GetPixel(x, y);

                            if (pixel != Color.FromArgb(255, 0, 0, 0))
                            {
                                var entity = new MapDTO.Entity();
                                entity.upperLeftCornerCoordinates = new Tuple<int, int>(x * 10, y * 10);
                                entity.lowerRightCornerCoordinate = FindEndingOfEntity(map, entity.upperLeftCornerCoordinates, MapDTO.EntityColorDictionary.FirstOrDefault(x => x.Value == pixel).Key);

                                var pictureBox = new PictureBox();
                                pictureBox.Location = new Point(entity.upperLeftCornerCoordinates.Item1, entity.upperLeftCornerCoordinates.Item2);
                                pictureBox.Size = new Size(entity.lowerRightCornerCoordinate.Item1+10 - entity.upperLeftCornerCoordinates.Item1, entity.lowerRightCornerCoordinate.Item2+10 - entity.upperLeftCornerCoordinates.Item2);
                                pictureBox.BackColor = pixel;
                                //pictureBox;

                                pictureBox.MouseClick += (sender, e) =>
                                {
                                    var pictureBox = sender as PictureBox;
                                    Debug.WriteLine($"Clicked on entity {entity.type} in X:{pictureBox.Location} with size of {pictureBox.Size}");
                                };

                                entity.pictureBox = pictureBox;

                                entity.type = MapDTO.EntityColorDictionary.FirstOrDefault(x => x.Value == pixel).Key;
                                entitiesInMap.Add(entity);

                                Debug.WriteLine($"Found entity {entity.type} in X:{entity.upperLeftCornerCoordinates.Item1} Y:{entity.upperLeftCornerCoordinates.Item2} with endings of X:{entity.lowerRightCornerCoordinate.Item1} Y:{entity.lowerRightCornerCoordinate.Item2}");
                            }
                        }
                    }
                    if(entitiesInMap.Count > 0)
                    {
                        listOfMaps.Add(entitiesInMap);
                        continue;
                    }
                    
                }
            }
            return listOfMaps;
        }

        public void UpdateSpaces(Tuple<int, int> startOfEntity, Tuple<int, int> endOfEntity)
        {
            for (int x = startOfEntity.Item1 / 10; x <= endOfEntity.Item1 / 10; x++)
            {
                for (int y = startOfEntity.Item2 / 10; y <= endOfEntity.Item2 / 10; y++)
                {
                    takenSpaces[x, y] = true;
                }
            }
        }

        public int AdjustEntity(Bitmap map, Tuple<int, int> startOfEntity, Tuple<int, int> endOfEntity, MapDTO.EntityType entityType)
        {
            var pixel = new Color();
            int smallestY = endOfEntity.Item2;
            
            for (int x = startOfEntity.Item1 / 10; x <= endOfEntity.Item1 / 10; x++)
            {
                for (int y = startOfEntity.Item2 / 10; y <= endOfEntity.Item2 / 10; y++)
                {
                    pixel = map.GetPixel(x, y);

                    if(pixel != MapDTO.EntityColorDictionary[entityType])
                    {
                        if(y < smallestY)
                        {
                            smallestY = (y - 1) * 10;
                            break;
                        }
                    }
                }
            }

            return smallestY;
        }

        public Tuple<int, int> FindEndingOfEntity(Bitmap map, Tuple<int, int> startOfEntity, MapDTO.EntityType entityType)
        {
            var pixel = new Color();

            int endingX = -1;
            int endingY = -1;

            for (int x = startOfEntity.Item1 / 10; x < map.Width; x++)
            {
                pixel = map.GetPixel(x, startOfEntity.Item2 / 10);

                if (pixel != MapDTO.EntityColorDictionary[entityType])
                {
                    endingX = (x - 1) * 10;
                    break;
                }
                else if(x == map.Width - 1)
                {
                    endingX = x * 10;
                    break;
                }
            }

            for (int y = startOfEntity.Item2 / 10; y < map.Height; y++)
            {
                pixel = map.GetPixel(endingX / 10, y);

                if (pixel != MapDTO.EntityColorDictionary[entityType])
                {
                    endingY = (y - 1) * 10;
                    break;
                }
                else if (y == map.Height - 1)
                {
                    endingY = y * 10;
                    break;
                }
            }

            //pixel = map.GetPixel(startOfEntity.Item1 / 10, ((startOfEntity.Item2 / 10) + (endingY / 20)) > 69 ? 69 : ((startOfEntity.Item2 / 10) + (endingY / 20)));


            if (map.GetPixel(startOfEntity.Item1 / 10, ((startOfEntity.Item2 / 10) + (endingY / 20)) > 69 ? 69 : ((startOfEntity.Item2 / 10) + (endingY / 20))) != MapDTO.EntityColorDictionary[entityType] ||
                map.GetPixel(endingX / 20, endingY / 10) != MapDTO.EntityColorDictionary[entityType] ||
                map.GetPixel(endingX / 20, endingY / 20) != MapDTO.EntityColorDictionary[entityType])
            {
                var temp = AdjustEntity(map, startOfEntity, new Tuple<int, int>(endingX, endingY), entityType);
                Debug.WriteLine($"Adjustment needed for entity. Adjusted ending Y from: {endingY} to {temp}");
                endingY = temp;
            }

            UpdateSpaces(startOfEntity, new Tuple<int, int>(endingX, endingY));

            return new Tuple<int, int>(endingX, endingY); ;
        }
    }
}
