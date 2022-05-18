using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BitMapAnalize
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "image3.bmp";
            string pc1 = "c1.bmp";
            //int[,] centr = { { 3, 1000, 800 }, { 200, 500, 500 } };
            
            Color[] c = { Color.Red, Color.Blue, Color.Green, Color.DarkCyan, Color.Purple, Color.DarkBlue, Color.Azure, Color.Crimson };
            ConsoleKeyInfo key;
            Bitmap image = new Bitmap(path);
            //Console.WriteLine(image.GetPixel(100, 100));
            Bitmap imageanal = new Bitmap(image.Width, image.Height);
            int[,] points = xypoints(image);
            do
            {
                try
                {
                    int[,] centr = Randmat(2, 3);
                    
                    //imageanal = analyz(points, centr, 80, imageanal, c);
                    imageanal = analyz(xypoints(image), centr, 80, imageanal, c);
                    imageanal.Save(pc1);
                    Console.WriteLine("All Done. Try Again?");
                    //key = Console.ReadKey();
                    key = new ConsoleKeyInfo('a', ConsoleKey.Enter, false, false, false);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("IndexOutOfRange. Try Again?");
                    key = Console.ReadKey();
                    //key = new ConsoleKeyInfo('a', ConsoleKey.Enter, false, false, false);
                }
                catch (DivideByZeroException)
                {
                    Console.WriteLine("DivideByZeroException. Try Again?");
                    key = Console.ReadKey();
                    //key = new ConsoleKeyInfo('a', ConsoleKey.Enter, false, false, false);

                }
            }
            while (key.Key == ConsoleKey.Enter);
        }
        public static int[,] Randmat(int x, int y)
        {
            DateTime d = DateTime.Now;
            int t = d.Millisecond + d.Second + d.Minute + d.Hour;
            Random r = new Random(t);
            int[,] mat = new int[x, y];
            for(int i = 0; i < x; i++)
            {
                for(int j = 0; j < y; j++)
                {
                    mat[i, j] = r.Next(100, 800);
                }
            }
            return mat;
        }
        public static Bitmap analyz(int[,] points, int[,] center, int eps, Bitmap img, Color[] c)
        {
            int k = center.GetLength(1); //xy - points
            int m = points.GetLength(0); //point - xy
            int[,] ncenter = new int[2, k]; //xy - points
            for (int i = 0; i < k; i++)
            {
                ncenter[0, i] = 0;
                ncenter[1, i] = 0;
            }
            int[,,] claspoints = new int[m,2,k]; //points - xy - claster
            int temp = 0;
            int ntemp = 1000000000;
            int classt = 0;
            int[] count = new int[k];
            for (int i = 0; i < k; i++)
            {
                count[i] = 0;
            }
            bool pr = true;
            int co = 0;
            bool[] prom = new bool[k];
            for (int i = 0; i < k; i++)
            {
                prom[i] = true;
            }
            while (pr)
            {
                for (int l = 0; l < k; l++)
                {
                    
                    
                        ncenter[0, l] = 0;
                        ncenter[1, l] = 0;
                    
                }
                ntemp = 1000000000;
                classt = 0;
                temp = 0;
                for(int i = 0; i < k; i++)
                {
                    count[i] = 0;
                }
                Console.Write('.');
                co++;
                for(int i = 0; i < m; i++)
                {
                    ntemp = 100000000;
                    for(int x = 0; x < k; x++)
                    {
                        if (prom[x])
                        {
                            temp = Convert.ToInt32(Math.Sqrt((points[i, 0] - center[0, x]) * (points[i, 0] - center[0, x]) + (points[i, 1] - center[1, x]) * (points[i, 1] - center[1, x])));
                            if (temp < ntemp)
                            {
                                classt = x;
                                ntemp = temp;
                            }
                        }
                    }
                    claspoints[count[classt], 0, classt] = points[i, 0];
                    claspoints[count[classt], 1, classt] = points[i, 1];
                    count[classt]++;
                    ntemp = 1000000000;
                }
                int temppx, temppy;
                for(int v = 0; v < k; v++)
                {
                    if (prom[v])
                    {
                        temppx = 0;
                        temppy = 0;
                        for (int u = 0; u < count[v]; u++)
                        {
                            temppx += claspoints[u, 0, v];
                            temppy += claspoints[u, 1, v];
                        }
                        try
                        {
                            ncenter[0, v] = temppx / count[v];
                            ncenter[1, v] = temppy / count[v];
                        }
                        catch (DivideByZeroException)
                        {
                            prom[v] = false;
                            ncenter[0, v] = 0;
                            ncenter[1, v] = 0;
                        }
                    }
                }
                
                pr = norm(center, ncenter) > eps;
                for(int i = 0; i < k; i++)
                {
                    center[0, i] = ncenter[0, i];
                    center[1, i] = ncenter[1, i];
                }
            }
            for(int a = 0; a < k; a++)
            {
                if (prom[a])
                {
                    for (int b = 0; b < count[a]; b++)
                    {
                        img.SetPixel(claspoints[b, 0, a], claspoints[b, 1, a], c[a]);
                    }
                }
            }
            Console.WriteLine(co);
            int colnum = 0;
            for(int i = 0; i < k; i++)
            {
                if (prom[i])
                    colnum++;
            }
            Console.WriteLine(colnum);
            return img;
        }
        public static int[,] searchPoints(Bitmap image)
        {

            int resx = image.Width;
            int resy = image.Height;
            int[,] point = new int[resx, resy];
            for (int i = 1; i <= resx; i++)
            {
                for (int j = 1; j <= resy; j++)
                {
                    if (image.GetPixel(i, j) == Color.Black)
                    {
                        point[i, j] = 1;
                    }
                    else
                    {
                        point[i, j] = 0;
                    }
                }
            }
            return point;
        }
        public static int[,] xypoints(Bitmap image)
        {
            Color col = Color.FromArgb(255, 255, 255, 255);
            int resx = image.Width;
            int resy = image.Height;
            
            int k = 0;
            for (int i = 1; i < resx; i++)
            {
                for (int j = 1; j < resy; j++)
                {
                    if (image.GetPixel(i, j) != col)
                    {
                        k++;
                    }
                }
            }
            int[,] point = new int[k, 2];
            int m = 0;
            for (int i = 1; i < resx; i++)
            {
                for (int j = 1; j < resy; j++)
                {
                    if (image.GetPixel(i, j) != col)
                    {
                        point[m, 0] = i;
                        point[m++, 1] = j;
                    }
                }
            }
            return point;
        }
        static int norm(int[,] X, int[,] newX)
        {
            int n = 0;
            int temp;
            for (int j = 0; j < X.GetLength(1); j++)
            {
                temp = 0;
                for(int i =0; i < X.GetLength(0); i++)
                {
                    temp += Math.Abs(X[i, j] - newX[i, j]);
                }
                if(temp > n)
                {
                    n = temp;
                }
            }
            return n;
        }
    }
   
}
