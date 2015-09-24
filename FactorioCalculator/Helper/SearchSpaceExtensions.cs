using FactorioCalculator.Models;
using FactorioCalculator.Models.PlaceRoute;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorioCalculator.Helper
{
    static class SearchSpaceExtensions
    {
        public static Image Draw(this SearchSpace space)
        {
            int cellSize = 64;
            Bitmap result = new Bitmap((int)space.Size.X * cellSize, (int)space.Size.Y * cellSize);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Black);
                var buildingBrush = new SolidBrush(Color.LightGray);

                foreach (var item in space.Buildings)
                {
                    var icon = Image.FromFile(item.Building.IconPath);
                    var destination = new Rectangle((int)item.Position.X * cellSize,
                        (int)item.Position.Y * cellSize, (int)item.Size.X * cellSize,
                        (int)item.Size.Y * cellSize);
                    
                    g.FillRectangle(buildingBrush, destination);
                    g.DrawImage(icon, destination);
                }
            }

            return result;
        }
    }
}
