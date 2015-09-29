using FactorioCalculator.Models;
using FactorioCalculator.Models.Factory;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static Image Draw(this Searchspace space)
        {
            int cellSize = 32;
            Bitmap result = new Bitmap((int)space.Size.X * cellSize, (int)space.Size.Y * cellSize);
            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Black);
                using (var buildingBrush = new SolidBrush(Color.LightGray))
                using (var flowPen = new Pen(Color.Red))
                using (var hiddenFlow = new Pen(Color.LightSalmon))
                using (var rotationPen = new Pen(Color.DarkGreen))
                {

                    foreach (var item in space.Buildings)
                    {
                        var center = (item.Position + item.Size / 2) * cellSize;

                        var currentFlowPen = hiddenFlow;
                        if (item.Building.IconPath != null)
                        {
                            var destination = new Rectangle((int)item.Position.X * cellSize,
                            (int)item.Position.Y * cellSize, (int)item.Size.X * cellSize,
                            (int)item.Size.Y * cellSize);

                            g.FillRectangle(buildingBrush, destination);

                            var icon = Image.FromFile(item.Building.IconPath);
                            g.DrawImage(icon, destination);

                            var direction = item.Rotation.ToVector() / 2 * cellSize + center;
                            g.DrawLine(rotationPen, (float)center.X, (float)center.Y, (float)direction.X, (float)direction.Y);

                            currentFlowPen = flowPen;
                        }

                        foreach (var prev in item.Previous)
                        {
                            var building = prev as IPhysicalBuilding;
                            if (building != null)
                            {
                                var from = (building.Position + building.Size / 2) * cellSize;

                                g.DrawLine(currentFlowPen, (float)from.X, (float)from.Y, (float)center.X, (float)center.Y);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
