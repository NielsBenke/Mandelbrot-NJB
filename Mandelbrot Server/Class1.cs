using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Mandelbrot_Server
{
    class Mandelbrot
    {
        public static int calc_iterations(Complex c, int maxiterations)
        {
            Complex z = new Complex(0, 0);
            int k;

            for (k = 0; k < maxiterations; k++)
            {
                z = Complex.Pow(z, 2) + c;
                if (z.Magnitude > 2)
                {
                    break;
                }
            }
            return k;
        }
    }
}
