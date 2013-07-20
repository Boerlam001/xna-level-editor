using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EditorModel
{
    public class Helper2d
    {
        public static void LinearEquationFrom2Points(Vector2 p1, Vector2 p2, out float a, out float b, out float c)
        {
            a = p2.Y - p1.Y;
            b = -(p2.X - p1.X);
            c = -(p2.Y - p1.Y) * p1.X + (p2.X - p1.X) * p1.Y;
            if (a < 0)
            {
                a *= -1;
                b *= -1;
                c *= -1;
            }
        }

        public static void LinearEquationFromGradientAndPoint(float m, Vector2 p, out float a, out float b, out float c)
        {
            a = m;
            b = -1;
            c = -m * p.X + p.Y;
            if (a < 0)
            {
                a *= -1;
                b *= -1;
                c *= -1;
            }
        }

        public static void PointOfTwoLine(float a1, float b1, float c1, float a2, float b2, float c2, out float x, out float y)
        {
            #region desc
            /*
             *  I:
             *  a1*x + b1*y + c1 = 0;
             *  y = (-a1*x - c1) / b1;
             *  
             *  II:
             *  a2*x + b2*y + c2 = 0;
             *  a2*x + b2*((-a1*x - c1) / b1) + c2 = 0;
             *  a2*x - (b2*a1*x/b1) - (b2*c1/b1) + c2 = 0;
             *  (a2 - (b2*a1/b1))*x - (b2*c1/b1) + c2 = 0;
             */
            #endregion

            x = ((b2 * c1 / b1) - c2) / (a2 - (b2 * a1 / b1));
            y = (-a1 * x - c1) / b1;
        }
    }
}
