using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CannyEdgeDetectionDemo
{
    public partial class MainForm : Form
    {
        public enum IsEdge { Yes, Maybe, No };

        public struct MyPixel
        {
            public int y;
            public int smoothY;
            public int gradientX;
            public int gradientY;
            public IsEdge isEdge;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private double[] Y = new double[] { 0.299, 0.587, 0.114 };

        private EdgeDetectionForm EdgeForm;

        private Bitmap originalBitmap;

        private void LoadImageBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openImageDlg = new OpenFileDialog())
            {
                openImageDlg.Title = "Load Image";
                openImageDlg.Filter = "Image Files |*.bmp;*.jpg;*.jpeg;*.gif;*.png";

                if (openImageDlg.ShowDialog() == DialogResult.OK)
                {
                    if (originalBitmap != null)
                    {
                        originalBitmap.Dispose();
                        originalBitmap = null;
                    }
                    originalBitmap = new Bitmap(openImageDlg.FileName);
                    if (originalBitmap.Height < 5 || originalBitmap.Width < 5)
                    {
                        MessageBox.Show("Selected image is too small. Please provide an image that is at least 5px by 5px.",
                            "Error: Image too small.",
                            MessageBoxButtons.OK);
                        originalBitmap.Dispose();
                        originalBitmap = null;
                    }
                    else
                    {
                        LoadedImageBox.LoadAsync(openImageDlg.FileName);
                    }
                }
            }
        }

        private int SmoothPixel(int[][] block)
        {
            int onesSum = block[0][0] + block[0][4] + block[4][0] + block[0][4];
            int foursSum = (block[0][1] + block[0][3] + block[1][0] + block[1][4] + block[3][0] + block[3][4] + block[4][1] + block[4][3]) * 4;
            int sevensSum = (block[0][2] + block[2][0] + block[2][4] + block[4][2]) * 7;
            int sixteensSum = (block[1][1] + block[3][1] + block[1][3] + block[3][3]) * 16;
            int twentySixesSum = (block[1][2] + block[2][1] + block[2][3] + block[3][2]) * 26;
            int middleSum = block[2][2] * 41;

            return (onesSum + foursSum + sevensSum + sixteensSum + twentySixesSum + middleSum + 137) / 273;
        }

        private void LoadedImageBox_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // if there is no bitmap, don't do anything
            if (originalBitmap == null)
            {
                return;
            }

            int width = originalBitmap.Width;
            int height = originalBitmap.Height;

            // initialize MyPixel matrix
            MyPixel[][] pixelMatrix = new MyPixel[height][];
            for (int row = 0; row < height; row++)
            {
                pixelMatrix[row] = new MyPixel[width];
            }

            // Start detection
            // for each pixel
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Color pixelColor = originalBitmap.GetPixel(col, row);

                    // put Y value into 2D matrix
                    pixelMatrix[row][col].y = (int)(Y[0] * pixelColor.R +
                        Y[1] * pixelColor.G +
                        Y[2] * pixelColor.B);
                }
            }

            // Pad yMatrix
            int paddedHeight = height + 4;
            int paddedWidth = width + 4;
            int[][] paddedYMatrix = new int[paddedHeight][];
            for (int row = 0; row < paddedYMatrix.Length; row++)
            {
                paddedYMatrix[row] = new int[paddedWidth];
            }

            // Inner paddedYMatrix
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    paddedYMatrix[row + 2][col + 2] = pixelMatrix[row][col].y;
                }
            }

            // Four Corners
            // Top Left Corner
            paddedYMatrix[0][0] = pixelMatrix[1][1].y;
            paddedYMatrix[0][1] = pixelMatrix[1][0].y;
            paddedYMatrix[1][0] = pixelMatrix[0][1].y;
            paddedYMatrix[1][1] = pixelMatrix[0][0].y;
            // Top Right Corner
            paddedYMatrix[0][paddedWidth - 1] = pixelMatrix[1][width - 2].y;
            paddedYMatrix[0][paddedWidth - 2] = pixelMatrix[1][width - 1].y;
            paddedYMatrix[1][paddedWidth - 1] = pixelMatrix[0][width - 2].y;
            paddedYMatrix[1][paddedWidth - 2] = pixelMatrix[0][width - 1].y;
            // Bottom Right Corner
            paddedYMatrix[paddedHeight - 1][paddedWidth - 1] = pixelMatrix[height - 2][width - 2].y;
            paddedYMatrix[paddedHeight - 1][paddedWidth - 2] = pixelMatrix[height - 2][width - 1].y;
            paddedYMatrix[paddedHeight - 2][paddedWidth - 1] = pixelMatrix[height - 1][width - 2].y;
            paddedYMatrix[paddedHeight - 2][paddedWidth - 2] = pixelMatrix[height - 1][width - 1].y;
            // Bottom Left Corner
            paddedYMatrix[paddedHeight - 1][0] = pixelMatrix[height - 2][1].y;
            paddedYMatrix[paddedHeight - 1][1] = pixelMatrix[height - 2][0].y;
            paddedYMatrix[paddedHeight - 2][0] = pixelMatrix[height - 1][1].y;
            paddedYMatrix[paddedHeight - 2][1] = pixelMatrix[height - 1][0].y;

            // Four Edges
            // Top & Bottom Edge
            for (int col = 0; col < width; col++)
            {
                // Very Top Edge
                paddedYMatrix[0][col + 2] = pixelMatrix[1][col].y;
                // Second From Top
                paddedYMatrix[1][col + 2] = pixelMatrix[0][col].y;

                // Second From Bottom
                paddedYMatrix[paddedHeight - 2][col + 2] = pixelMatrix[height - 1][col].y;
                // Very Bottom Edge
                paddedYMatrix[paddedHeight - 1][col + 2] = pixelMatrix[height - 2][col].y;
            }
            // Left and Right Edges
            for (int row = 0; row < width; row++)
            {
                if (row != 0 || row != 1 || row != width - 2 || row != width - 1)
                {
                    continue;
                }
                paddedYMatrix[row + 2][0] = pixelMatrix[row][1].y;
                paddedYMatrix[row + 2][1] = pixelMatrix[row][0].y;

                paddedYMatrix[row + 2][paddedWidth - 2] = pixelMatrix[row][width - 1].y;
                paddedYMatrix[row + 2][paddedWidth - 1] = pixelMatrix[row][width - 2].y;
            }

            // Smooth the pixels
            for (int row = 2; row < paddedHeight - 2; row++)
            {
                for (int col = 2; col < paddedWidth - 2; col++)
                {
                    int[][] arr = new int[5][];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = paddedYMatrix[i + row - 2].Skip(col - 2).Take(5).ToArray();
                    }
                    pixelMatrix[row - 2][col - 2].smoothY = SmoothPixel(new int[][]
                    {
                        paddedYMatrix[row - 2].Skip(col-2).Take(5).ToArray(),
                        paddedYMatrix[row - 1].Skip(col-2).Take(5).ToArray(),
                        paddedYMatrix[row].Skip(col-2).Take(5).ToArray(),
                        paddedYMatrix[row + 1].Skip(col-2).Take(5).ToArray(),
                        paddedYMatrix[row + 2].Skip(col-2).Take(5).ToArray(),
                    });
                }
            }

            // Calculate the gradient X value
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int prev, next;
                    if (col == 0)
                    {
                        prev = pixelMatrix[row][col + 1].smoothY;
                    }
                    else
                    {
                        prev = pixelMatrix[row][col - 1].smoothY;
                    }
                    if (col == width - 1)
                    {
                        next = pixelMatrix[row][col - 1].smoothY;
                    }
                    else
                    {
                        next = pixelMatrix[row][col + 1].smoothY;
                    }

                    pixelMatrix[row][col].gradientX = (next - prev) / 2;
                }
            }

            // Calculate the gradient Y value
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int prev, next;
                    if (row == 0)
                    {
                        prev = pixelMatrix[row + 1][col].smoothY;
                    }
                    else
                    {
                        prev = pixelMatrix[row - 1][col].smoothY;
                    }
                    if (row == height - 1)
                    {
                        next = pixelMatrix[row - 1][col].smoothY;
                    }
                    else
                    {
                        next = pixelMatrix[row + 1][col].smoothY;
                    }

                    pixelMatrix[row][col].gradientY = (next - prev) / 2;
                }
            }

            // Find is Max by figuring the gradient and finding its relation to other pixels
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int rowPlusOne, rowMinusOne, colPlusOne, colMinusOne;
                    if (row == 0)
                    {
                        rowMinusOne = row + 1;
                    }
                    else
                    {
                        rowMinusOne = row - 1;
                    }
                    if (row == height - 1)
                    {
                        rowPlusOne = row - 1;
                    }
                    else
                    {
                        rowPlusOne = row + 1;
                    }
                    if (col == 0)
                    {
                        colMinusOne = col + 1;
                    }
                    else
                    {
                        colMinusOne = col - 1;
                    }
                    if (col == width - 1)
                    {
                        colPlusOne = col - 1;
                    }
                    else
                    {
                        colPlusOne = col + 1;
                    }

                    int x = pixelMatrix[row][col].gradientX;
                    int y = pixelMatrix[row][col].gradientY;
                    if (x > 0 && y > 0)
                    {
                        // In quadrant A
                        if (y > x)
                        {
                            // Test against ([row-1][col] + [row-1][col+1]) / 2 and ([row+1][col] + [row+1][col-1]) / 2
                            int smoothY1 = (pixelMatrix[rowMinusOne][col].smoothY + pixelMatrix[rowMinusOne][colPlusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[rowPlusOne][col].smoothY + pixelMatrix[rowPlusOne][colMinusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                        else
                        {
                            // Test against ([row][col+1] + [row+1][col+1]) /2 and ([row][col-1] + [row-1][col-1]) / 2
                            int smoothY1 = (pixelMatrix[row][colPlusOne].smoothY + pixelMatrix[rowPlusOne][colPlusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[row][colMinusOne].smoothY + pixelMatrix[rowMinusOne][colMinusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                    }
                    else if (y > 0)
                    {
                        // In quadrant S
                        if (y > Math.Abs(x))
                        {
                            // Test against ([row-1][col] + [row-1][col-1]) / 2 and ([row+1][col] + [row+1][col+1]) / 2
                            int smoothY1 = (pixelMatrix[rowMinusOne][col].smoothY + pixelMatrix[rowMinusOne][colMinusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[rowPlusOne][col].smoothY + pixelMatrix[rowPlusOne][colPlusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                        else
                        {
                            // Test against ([row][col+1] + [row-1][col+1]) / 2 and ([row][col-1] + [row+1][col-1]
                            int smoothY1 = (pixelMatrix[row][colPlusOne].smoothY + pixelMatrix[rowMinusOne][colPlusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[row][colMinusOne].smoothY + pixelMatrix[rowPlusOne][colMinusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                    }
                    else if (x > 0)
                    {
                        // In quadrant C
                        if (Math.Abs(y) > x)
                        {
                            // Test against ([row-1][col] + [row-1][col-1]) / 2 and ([row+1][col] + [row+1][col+1]) / 2
                            int smoothY1 = (pixelMatrix[rowMinusOne][col].smoothY + pixelMatrix[rowMinusOne][colMinusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[rowPlusOne][col].smoothY + pixelMatrix[rowPlusOne][colPlusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                        else
                        {
                            // Test against ([row][col+1] + [row-1][col+1]) / 2 and ([row][col-1] + [row+1][col-1]
                            int smoothY1 = (pixelMatrix[row][colPlusOne].smoothY + pixelMatrix[rowMinusOne][colPlusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[row][colMinusOne].smoothY + pixelMatrix[rowPlusOne][colMinusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                    }
                    else
                    {
                        // In quadrant T
                        if (Math.Abs(y) > Math.Abs(x))
                        {
                            // Test against ([row-1][col] + [row-1][col+1]) / 2 and ([row+1][col] + [row+1][col-1]) / 2
                            int smoothY1 = (pixelMatrix[rowMinusOne][col].smoothY + pixelMatrix[rowMinusOne][colPlusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[rowPlusOne][col].smoothY + pixelMatrix[rowPlusOne][colMinusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                        else
                        {
                            // Test against ([row][col+1] + [row+1][col+1]) /2 and ([row][col-1] + [row-1][col-1]) / 2
                            int smoothY1 = (pixelMatrix[row][colPlusOne].smoothY + pixelMatrix[rowPlusOne][colPlusOne].smoothY) / 2;
                            int smoothY2 = (pixelMatrix[row][colMinusOne].smoothY + pixelMatrix[rowMinusOne][colMinusOne].smoothY) / 2;
                            int originalSmoothY = pixelMatrix[row][col].smoothY;
                            if (originalSmoothY > smoothY1 && originalSmoothY > smoothY2)
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Yes;
                            }
                            else
                            {
                                pixelMatrix[row][col].isEdge = IsEdge.Maybe;
                            }
                        }
                    }
                }
            }

            // evaluate top 5% and low 5%
            Dictionary<int, int> yHist = new Dictionary<int, int>();
            List<int> topYValues = new List<int>();
            List<int> lowYValues = new List<int>();

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int y = pixelMatrix[row][col].smoothY;
                    if (!yHist.ContainsKey(y))
                    {
                        yHist.Add(y, 0);
                    } else
                    {
                        yHist[y]++;
                    }
                }
            }

            Dictionary<int, int>.KeyCollection keyColl = yHist.Keys;
            List<int> sortedYValues = keyColl.ToList();
            sortedYValues.Sort(new Comparison<int>(
                (i1, i2) => yHist[i2].CompareTo(yHist[i1])));
            int numberOfTopOrLow = sortedYValues.Count / 100 * 5;
            for (int i = 0; i < numberOfTopOrLow; i++)
            {
                lowYValues.Add(sortedYValues[i]);
                topYValues.Add(sortedYValues[sortedYValues.Count - 1 - i]);
            }

            // Threshold top and low y values
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int y = pixelMatrix[row][col].smoothY;
                    if (lowYValues.Contains(y))
                    {
                        pixelMatrix[row][col].isEdge = IsEdge.No;
                    }
                    if (topYValues.Contains(y))
                    {
                        pixelMatrix[row][col].isEdge = IsEdge.Yes;
                    }
                }
            }

            // duplicate the MyPixel matrix
            IsEdge[][] edgeMatrix = new IsEdge[height][];
            for (int i = 0; i < height; i++)
            {
                edgeMatrix[i] = new IsEdge[width];

                for (int j = 0; j < width; j++)
                {
                    edgeMatrix[i][j] = pixelMatrix[i][j].isEdge;
                }
            }

            // Get rid of Maybes
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (pixelMatrix[row][col].isEdge == IsEdge.Maybe)
                    {
                        bool isTopEdge = row == 0;
                        bool isBottomEdge = row == height - 1;
                        bool isLeftEdge = col == 0;
                        bool isRightEdge = col == width - 1;

                        if (!isLeftEdge)
                        {
                            // do row and col - 1 check
                            if (pixelMatrix[row][col - 1].isEdge == IsEdge.Yes)
                            {
                                edgeMatrix[row][col] = IsEdge.Yes;
                                continue;
                            }
                        }
                        if (!isRightEdge)
                        {
                            // do row and col + 1 check
                            if (pixelMatrix[row][col + 1].isEdge == IsEdge.Yes)
                            {
                                edgeMatrix[row][col] = IsEdge.Yes;
                                continue;
                            }
                        }
                        if (!isTopEdge)
                        {
                            // do row - 1 and col check
                            if (pixelMatrix[row - 1][col].isEdge == IsEdge.Yes)
                            {
                                edgeMatrix[row][col] = IsEdge.Yes;
                                continue;
                            }
                            if (!isLeftEdge)
                            {
                                // do row - 1 and col - 1 check
                                if (pixelMatrix[row - 1][col - 1].isEdge == IsEdge.Yes)
                                {
                                    edgeMatrix[row][col] = IsEdge.Yes;
                                    continue;
                                }
                            }
                            if (!isRightEdge)
                            {
                                // do row - 1 and col + 1 check
                                if (pixelMatrix[row - 1][col + 1].isEdge == IsEdge.Yes)
                                {
                                    edgeMatrix[row][col] = IsEdge.Yes;
                                    continue;
                                }
                            }
                        }
                        if (!isBottomEdge)
                        {
                            // do row + 1 and col check
                            if (pixelMatrix[row + 1][col].isEdge == IsEdge.Yes)
                            {
                                edgeMatrix[row][col] = IsEdge.Yes;
                                continue;
                            }
                            if (!isLeftEdge)
                            {
                                // do row + 1 and col - 1 check
                                if (pixelMatrix[row + 1][col - 1].isEdge == IsEdge.Yes)
                                {
                                    edgeMatrix[row][col] = IsEdge.Yes;
                                    continue;
                                }
                            }
                            if (!isRightEdge)
                            {
                                // do row + 1 and col + 1 check
                                if (pixelMatrix[row + 1][col + 1].isEdge == IsEdge.Yes)
                                {
                                    edgeMatrix[row][col] = IsEdge.Yes;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }


            Bitmap newBitmap = new Bitmap(width, height);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    IsEdge isEdge = edgeMatrix[row][col];
                    if (isEdge == IsEdge.Yes)
                    {
                        newBitmap.SetPixel(col, row, Color.White);
                    }
                    else
                    {
                        newBitmap.SetPixel(col, row, Color.Black);
                    }
                }
            }

            if (EdgeForm == null)
            {
                EdgeForm = new EdgeDetectionForm();
                EdgeForm.FormClosed += (_, arg) =>
                {
                    EdgeForm = null;
                };
                EdgeForm.SetImage(newBitmap);
                EdgeForm.Show();
            }
            else
            {
                EdgeForm.SetImage(newBitmap);
            }
        }
    }
}
