

using Berzerk.DTOs;
using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BerzerkTests
{
    public class MapBuildTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public MapBuildTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InvalidDirectory()
        {
            Assert.Throws<FileNotFoundException>(() => new MapBuilder(Environment.CurrentDirectory + "\\..\\Sprites\\"));
        }
        [Fact]
        public void CreateMap_InvalidFormat()
        {
            MapDTO.MapWidth = 120;
            MapDTO.MapHeight = 70;
            var directory = Environment.CurrentDirectory + "\\..\\..\\..\\..\\Berzerk\\Sprites\\";
            _testOutputHelper.WriteLine(directory);
            var mapBuilder = new MapBuilder(directory);

            Assert.ThrowsAny<FileFormatException>(() => mapBuilder.CreateMaps());
        }
        [Fact]
        public void CreateMap_CheckMaps()
        {
            MapDTO.MapWidth = 125;
            MapDTO.MapHeight = 70;
            var directory = Environment.CurrentDirectory + "\\..\\..\\..\\..\\Berzerk\\Sprites\\";
            _testOutputHelper.WriteLine(directory);
            var mapBuilder = new MapBuilder(directory);
            var result = mapBuilder.CreateMaps();

            Assert.NotEmpty(result);
        }
    }
}
