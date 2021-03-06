﻿using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Threading.Tasks;
/* ---------------------- Added Libraries ---------------------- */
using BitMiracle.LibTiff.Classic; // Use Tiff images
using System.Threading; // PauseToken
using ManagedCuda.BasicTypes; // CUDA Libraries

namespace Project_LENA___WPF
{
    class Functions
    {
        MainWindow Window;

        // function constructor
        public Functions(MainWindow Args)
        {
            Window = Args;
        }

        // convert tiff image to 3d byte array
        public static byte[,,] Tiff2Array(Tiff image, int height, int width, int samples)
        {
            // store the image information in 2d byte array
            // reserve memory for storing the size of 1 line
            byte[] scanline = new byte[image.ScanlineSize()];
            // reserve memory for the size of image
            byte[,,] im = new byte[height, width, samples];
            for (int i = 0; i < height; i++)
            {
                image.ReadScanline(scanline, i);
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < samples; k++)
                    {
                        im[i, j, k] = scanline[(samples * j) + k];

                    }
                }
            } // end grabbing intensity values           
            return im;
        } // end method

        // write the image to file
        public void WriteToFile(byte[,,] im, int width, int height, byte bits, byte samples, double dpiX, double dpiY, string FileName)
        {
            // Attempt to recreate the image from 2d byte array im
            using (Tiff output = Tiff.Open(FileName, "w"))
            {
                // simple error check
                if (output == null)
                {
                    Window.SetText2("Can not read the image.\r\n");
                    return;
                }
                // set tag information
                #region SetTagInfo
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.BITSPERSAMPLE, bits);
                output.SetField(TiffTag.SAMPLESPERPIXEL, samples);
                output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                if (samples == 1) output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                else if (samples == 3) output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.ROWSPERSTRIP, 1);
                output.SetField(TiffTag.XRESOLUTION, dpiX);
                output.SetField(TiffTag.YRESOLUTION, dpiY);
                output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);
                #endregion

                // reserve buffer
                byte[] buffer = new byte[width * samples * sizeof(byte /*can be changed depending on the format of the image*/)];
                // obtain each line of the final byte arrays and write them to a file
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < samples; k++)
                        {
                            buffer[samples * j + k] = im[i, j, k];
                        }
                    }
                    // write
                    if (samples == 1) output.WriteScanline(buffer, i);
                    else if (samples == 3) output.WriteEncodedStrip(i, buffer, samples * width);
                }
                // write to file
                output.WriteDirectory();
                output.Dispose();
                //System.Diagnostics.Process.Start(saveFileDialog2.FileName); // displays the result
            }// end inner using
        }

        // create surrounding borders
        public byte[,,] MirrorImage(byte[,,] im, int height, int width, int samples, int offset)
        {
            // write code here... someday
            // Code has been written here... Hurray!

            // reserving 3D array with extended sizes
            int mirroredHeight = height + (offset * 2);
            int mirroredWidth = width + (offset * 2);
            byte[,,] image = new byte[mirroredHeight, mirroredWidth, samples];

            Window.SetText2("Calling MirrorImage... Done.\r\nSize of new matrix is " + mirroredWidth + " by " + mirroredHeight + Environment.NewLine + Environment.NewLine);
            
            // copy original image
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < samples; k++)
                    {
                        image[i + offset, j + offset, k] = im[i, j, k];
                    }
                }
            }
            // copy columns - confirmed to work
            for (int i = 0; i < offset; i++) // 0~3 samples per pixel if mirrorImage
            {
                for (int col = offset; col < offset + height; col++) // 3~514 Height of mirrorImage
                {
                    for (int k = 0; k < samples; k++)
                    {
                        // copy left columns
                        image[col, i, k] = im[col - offset, offset - i, k];
                        // copy right columns
                        image[col, i + width + offset, k] = im[col - offset, width - i - 2, k];
                    } // end for
                } // end for
            } // end for

            for (int i = 0; i < offset; i++) // 0~2 Height of mirrorImage
            {
                for (int row = 0; row < width + (offset * 2); row++) // 0~514 Width of mirrorImage
                {
                    for (int k = 0; k < samples; k++)
                    {
                        // copy top rows
                        image[i, row, k] = image[(offset * 2) - i, row, k];
                        // copy bottom rows
                        image[i + height + offset, row, k] = image[height + offset - 2 - i, row, k];
                    }
                }
            }
            return image;
        }

        // create kernel window using pixels
        public static byte[,] CreateWindow(byte[,,] im, int row, int col, int sample, int kernel, int offset)
        {
            byte[,] image = new byte[kernel, kernel];
            for (int i = 0; i < kernel; i++)
            {
                for (int j = 0; j < kernel; j++)
                {
                    image[i, j] = im[row - offset + i, col - offset + j, sample];
                }
            }
            return image;
        }

        // create kernel window using patches
        public static byte[,] CreatePatch(byte[,,] im, int row, int col, int sample, int kernel)
        {
            byte[,] image = new byte[kernel, kernel];
            for (int i = 0; i < kernel; i++)
            {
                for (int j = 0; j < kernel; j++)
                {
                    //if (im.le
                    image[i, j] = im[row + i, col + j, sample];

                }
            }
            return image;
        }

        // create kernel window using patches as an array
        public static byte[] CreatePatchAsArray(byte[,,] im, int row, int col, int sample, int kernel)
        {
            byte[] array = new byte[kernel * kernel];
            for (int i = 0; i < kernel; i++)
            {
                for (int j = 0; j < kernel; j++)
                {
                    array[(i * kernel) + j] = im[row + i, col + j, sample];
                }
            }
            return array;
        }

        // create learning samples using pixels
        public static void LearnSet(byte[,,] clean, byte[,,] noised, int kernel, int sSize, string FileName)
        {

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName, true))
            {
                int offset = (kernel - 3) / 2 + 1;
                // get height and width
                int cleanHeight = clean.GetLength(0);
                int cleanWidth = clean.GetLength(1);
                int samples = clean.GetLength(2);
                // write the number of samples to file
                file.WriteLine(sSize.ToString());
                // initialize random number generator
                Random random = new Random();
                // begin generating samples
                for (int v = 0; v < sSize; v++)
                {
                    // generate random coordinate
                    int randomRow = random.Next(0, 256);
                    int randomCol = random.Next(0, 256);
                    int randomSample = random.Next(0, samples);
                    // fetch kernel
                    byte[,] inputArray = CreateWindow(noised, randomRow + offset, randomCol + offset, randomSample, kernel, offset);
                    // get optimal intensity value from clean image
                    int pixel = clean[randomRow, randomCol, randomSample];
                    // reserve byte array
                    byte[] S = new byte[kernel * kernel];
                    // transform multi dimensional inputArray to 1d array
                    for (int i = 0; i < kernel; i++)
                    {
                        for (int j = 0; j < kernel; j++)
                        {
                            S[kernel * i + j] = inputArray[i, j];
                        } // end for loop
                    } // end for loop
                    // write to file
                    for (int i = 0; i < S.Length; i++)
                    {
                        file.Write(S[i] + " ");
                    }
                    file.Write(pixel);
                    file.WriteLine();
                } // end for loop
                file.Dispose();
            } // end using scope

        }

        // create learning samples using patches
        public static void LearnSetPatch(byte[,,] clean, byte[,,] noised, int kernel, int Size, string FileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName, true)) // Samples.txt
            {
                // get height and width
                int patchLength = kernel * kernel;
                int twoPatchLength = patchLength * 2;
                int rowSize = clean.GetLength(0);  //rowSize
                int colSize = clean.GetLength(1);   //colSize
                int samples = clean.GetLength(2);    //sampleSize
                double temp = rowSize / kernel;
                int sSizeRow = (int)Math.Floor(temp) - 1;
                temp = colSize / kernel;
                int sSizeCol = (int)Math.Floor(temp) - 1;
                int sSize = sSizeRow * sSizeCol;
                int sSizeRowIndex = sSizeRow * kernel;
                int sSizeColIndex = sSizeCol * kernel;

                // initialize variables to store patches
                byte[] noisyVector = new byte[kernel * kernel];
                byte[] cleanVector = new byte[kernel * kernel];
                byte[,,] vectorMAP = new byte[samples, sSize, twoPatchLength];
                Random random = new Random();

                // generate all possible patches first
                for (int k = 0; k < samples; k++)
                {
                    for (int row = 0; row < sSizeRow; row++)
                    {
                        for (int col = 0; col < sSizeCol; col++)
                        {
                            //create patch from noisy image
                            noisyVector = CreatePatchAsArray(noised, row * kernel, col * kernel, k, kernel);
                            cleanVector = CreatePatchAsArray(clean, row * kernel, col * kernel, k, kernel);
                            // store in vectorMap
                            // noisy first
                            for (int v = 0; v < patchLength; v++)
                                vectorMAP[k, row * sSizeCol + col, v] = noisyVector[v];
                            // then clean
                            for (int v = patchLength; v < twoPatchLength; v++)
                                vectorMAP[k, row * sSizeCol + col, v] = cleanVector[v - patchLength];
                        }
                    }
                }
                byte[] S = new byte[twoPatchLength];
                // done generating all possible patches.
                // Now we need to randomly select patches
                for (int v = 0; v < Size; v++)
                {
                    //generate random coordinate
                    int randomRow = random.Next(0, sSize);
                    int randomSample = random.Next(0, samples);
                    //write to file
                    for (int i = 0; i < twoPatchLength; i++)
                        S[i] = vectorMAP[randomSample, randomRow, i];
                    for (int i = 0; i < twoPatchLength; i++)
                    {
                        //file.Write(vectorMAP[randomRow, i]);
                        file.Write(S[i] + " ");
                    }
                    file.WriteLine();
                } // end for loop
                file.Dispose();
            } // end using scope
        }

        // create gaussian noise
        public static byte[,,] createRandomTiff(int width, int height, double mean, double standarddev, double noise, byte[,,] grey, bool? checkBox1, int nsample)
        {
            #region variable initialization
            int samples = grey.GetLength(2);    //sampleSize
            double m = mean;
            double σ = standarddev; // Greek letter sigma
            Random φ = new Random(); // Greek letter phi
            Random Γ = new Random();// Greek letter gamma

            // sine and cosine variables for the Box-Muller algorithm
            double[,,] z1 = new double[height, width, samples];
            double[,,] z2 = new double[height, width, samples];

            // normally distributed variables gathered from Box-Muller algorithm with added image mean and sigma
            double[,,] x1 = new double[height, width, samples];
            double[,,] x2 = new double[height, width, samples];

            double[,,] g = new double[height, width, samples];

            double number; // used to fix bug
            #endregion

            // applying Gaussian noise to each pixel
            for (int k = 0; k < nsample; k++)
            {
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        // the Box-Muller algorithm
                        z1[i, j, k] = Math.Cos(2 * Math.PI * φ.NextDouble()) * Math.Sqrt(-2 * Math.Log(Γ.NextDouble()));
                        z2[i, j, k] = Math.Sin(2 * Math.PI * φ.NextDouble()) * Math.Sqrt(-2 * Math.Log(Γ.NextDouble()));

                        number = φ.NextDouble(); // fixes bug (for some reason)

                        x1[i, j, k] = m + z1[i, j, k] * noise * σ;
                        x2[i, j, k] = m + z2[i, j, k] * noise * σ;

                        #region Polar form of the Box-Muller algorithm
                        //do
                        //{
                        //    x3[i, j] = 2 * φ.NextDouble() - 1;
                        //    x4[i, j] = 2 * Γ.NextDouble() - 1;
                        //    w[i, j] = x3[i, j] * x3[i, j] + x4[i, j] * x4[i, j];
                        //} while (w[i, j] >= 1);

                        //w[i, j] = Math.Sqrt((-2 * Math.Log(w[i, j])) / w[i, j]);
                        //z1[i, j] = x3[i, j] * w[i, j];
                        //z2[i, j] = x4[i, j] * w[i, j];

                        //x1[i, j] = m + z1[i, j] * noise * σ;
                        //x2[i, j] = m + z2[i, j] * noise * σ;
                        #endregion
                    }
                }
            }
            #region Checkbox status
            if (checkBox1 == true) // adds noise to image
            {
                for (int k = 0; k < samples; k++)
                {
                    for (int i = 0; i < height; ++i)
                    {
                        for (int j = 0; j < width; ++j)
                        {
                            if (j % 2 != 0)
                                g[i, j, k] = grey[i, j, k] + x1[i, j, k] - m;
                            if (j % 2 == 0)
                                g[i, j, k] = grey[i, j, k] + x2[i, j, k] - m;

                            if (g[i, j, k] > 255) g[i, j, k] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                            if (g[i, j, k] < 0) g[i, j, k] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                        }
                    }
                }
            }

            else if (checkBox1 == false) // only displays noise
            {
                for (int k = 0; k < samples; k++)
                {
                    for (int i = 0; i < height; ++i)
                    {
                        for (int j = 0; j < width; ++j)
                        {
                            if (j % 2 != 0)
                                g[i, j, k] = x1[i, j, k];
                            if (j % 2 == 0)
                                g[i, j, k] = x2[i, j, k];

                            if (g[i, j, k] > 255) g[i, j, k] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                            if (g[i, j, k] < 0) g[i, j, k] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                        }
                    }
                }
            }
            #endregion


            byte[,,] test = new byte[height, width, samples];

            for (int k = 0; k < samples; k++)
                {
                    for (int i = 0; i < height; ++i)
                    {
                        for (int j = 0; j < width; ++j)
                        {
                            test[i, j, k] = Convert.ToByte(g[i, j, k]);
                        }
                    }
            }

            return test;

            #region old code
            //for (int i = 0; i < height; ++i)
            //{
            //    byte[] buf = new byte[width];
            //    for (int j = 0; j < width; ++j)
            //        buf[j] = (byte)random.Next(255);

            //    output.WriteScanline(buf, i);
            //}
            #endregion
        }
    }

    #region PauseToken
    public class PauseTokenSource
    {
        public bool IsPaused
        {
            get { return m_paused != null; }
            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(
                        ref m_paused, new TaskCompletionSource<bool>(), null);
                }
                else
                {
                    while (true)
                    {
                        var tcs = m_paused;
                        if (tcs == null) return;
                        if (Interlocked.CompareExchange(ref m_paused, null, tcs) == tcs)
                        {
                            tcs.SetResult(true);
                            break;
                        }
                    }
                }
            }
        }

        public PauseToken Token { get { return new PauseToken(this); } }

        private volatile TaskCompletionSource<bool> m_paused;

        internal Task WaitWhilePausedAsync()
        {
            var cur = m_paused;
            return cur != null ? cur.Task : s_completedTask;
        }

        internal static readonly Task s_completedTask = Task.FromResult(true);
    }

    public struct PauseToken
    {
        private readonly PauseTokenSource m_source;
        internal PauseToken(PauseTokenSource source) { m_source = source; }

        public bool IsPaused { get { return m_source != null && m_source.IsPaused; } }

        public Task WaitWhilePausedAsync()
        {
            return IsPaused ?
                m_source.WaitWhilePausedAsync() :
                PauseTokenSource.s_completedTask;
        }
    }
    #endregion
}

