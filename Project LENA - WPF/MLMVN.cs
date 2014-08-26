using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Threading.Tasks;
/* ---------------------- Added Libraries ---------------------- */
using System.Numerics; // Complex numbers
using System.Threading; // CancellationToken
using System.IO; // BinaryReader, open and save files

namespace Project_LENA___WPF
{
    class MLMVN
    {
        MainWindow window;
        Functions functions;

        public MLMVN(MainWindow function)
        {
            window = function;
            functions = new Functions(function);
        }

        /* -------------------------- Denoise by Pixels ----------------------------------------------------------------- */

        public async Task<byte[,]> Activation(byte[,] noisyImage, int kernel, string weights, int numberofsectors, int inLayerSize, int hidLayerSize, CancellationToken cancelToken, PauseToken pauseToken)
        {
            // get height and width
            int height = noisyImage.GetLength(0);
            int width = noisyImage.GetLength(1);
            int offset;
            switch (kernel)
            {
                case 3:
                    offset = 1;
                    break;
                case 5:
                    offset = 2;
                    break;
                case 7:
                    offset = 3;
                    break;
                default:
                    offset = 0;
                    break;
            } // end switch
            if (offset == 0)
            {
                window.SetText1("Value of kernel is not properly set." + Environment.NewLine);
                return null;
            }

            // extend the image
            byte[,] image = new byte[height + offset * 2, width + offset * 2];
            image = functions.MirrorImage(noisyImage, height, width, offset);

            window.SetProgress1(4);

            // pre-instantiate complex 2d-array
            double[,] inputArray = new double[kernel, kernel];
            var CinputArray = new Complex[kernel, kernel];
            // instantiate imaginary unit
            var complex1 = new Complex(0.0, 1.0);
            // pass to neural network

            byte[,] src;

            //int FirstQ = offset;
            //Parallel.For(FirstQ, height + offset, Q =>
            for (int Q = offset; Q < height + offset; Q++)
            {

                for (int P = offset; P < width + offset; P++)
                {
                    src = Functions.CreateWindow(image, Q, P, kernel, offset);
                    Array.Copy(src, inputArray, src.Length);
                    // transformation of inputs into complex plane
                    for (int i = 0; i < kernel; i++)
                    {
                        for (int j = 0; j < kernel; j++)
                            CinputArray[i, j] = Exp(complex1 * 2 * Math.PI * inputArray[i, j] / numberofsectors);
                    } // end nested for loop
                    // process
                    noisyImage[Q - offset, P - offset] = NeuralNetwork(CinputArray, weights, numberofsectors, inLayerSize, hidLayerSize);
                }
                // Action when cancel button is clicked
                if (cancelToken.IsCancellationRequested)
                    cancelToken.ThrowIfCancellationRequested();

                // Action when pause button is clicked
                await pauseToken.WaitWhilePausedAsync();

                //Increments progress bar
                window.SetProgress1(1);

            };//);  
            return noisyImage;
        }// end method

        public byte NeuralNetwork(Complex[,] inputArray, string weightName, int numberofsectors, int inLayerSize, int hidLayerSize)
        {
            /*
             * -------------------------- Used Varianles --------------------------------------------
             * int inSize - length of kernel: if kernel is 3x3, then it's 3
             * int weightSize - number of doubles to read for one complete weight: if 3x3 kernel, 9(3x3) + w0(first weight) = 10 x 2(for real and imaginary) = 20
             * Complex [] S - 1d array that stores contents of kernel with x0 = complex(1,1) to match the size of array with weights
             * double [] weight - 1d double array that stores values of weights read from a file
             * Complex [] Ilary - first layer
             * COmplex [] Olary - second layer
             * BinaryReader b - file stream to read double values from a weight file
             * 
             */

            // ------------------- Variable Initialization
            #region Variable Initialization
            // length of kernel
            int inSize = (int)System.Math.Sqrt(inputArray.Length);
            // if 3x3, 9 weights plus w0 times 2
            int weightSize = (inputArray.Length + 1) * 2;
            // S = kernel array
            // transform to 1d-array, as well as take extra w0 into account
            Complex[] S = new Complex[inputArray.Length + 1];
            // initialize x0 as 1
            S[0] = new Complex(1, 0);
            // copy
            for (int i = 0; i < inSize; i++)
                for (int j = 0; j < inSize; j++)
                    S[i * inSize + j + 1] = inputArray[i, j];
            // end for loop

            // read weights
            double[] weight = new double[weightSize];
            double[] hidWeight = new double[(inLayerSize + 1) * 2];
            Complex[] Ilary = new Complex[inLayerSize];
            Complex[] Olary = new Complex[hidLayerSize];
            // open file stream
            BinaryReader b = new BinaryReader(File.Open(weightName, FileMode.Open));
            // indicates positon of file 
            int pos = 0;

            #endregion End of Variable Initialization
            // ------------------ End of Variable Initialization

            // ----------------- Building input layer
            #region Input Layer
            // generate weighted sum of the inner layer
            for (int B = 0; B < inLayerSize; B++)
            {
                // Position and length variables.
                pos = 0;
                while (pos < weightSize * sizeof(double))
                {
                    // Read integer.
                    weight[pos / sizeof(double)] = b.ReadDouble();
                    // Advance our position variable
                    pos += sizeof(double);
                }
                // process neurons in the inner layer
                Ilary[B] = Neuron(S, weight);
            } // end for loop

            // Normalization
            for (int i = 0; i < inLayerSize; i++)
                Ilary[i] /= Complex.Abs(Ilary[i]);
            // end for loop
            #endregion
            //------------------ End of input layer

            // ----------------- Building hidden layer
            #region Hidden Layer
            // this part executes oly if the network contains hidden layers.
            // Olary would contain the weighted sum of the output
            if (hidLayerSize != 0)
            {
                // (3+1)*2 = 8
                int newWeight = (Ilary.Length + 1) * 2;
                for (int B = 0; B < hidLayerSize; B++)
                {
                    // Position and length variables.
                    pos = 0;
                    while (pos < newWeight * sizeof(double))
                    {
                        // Read double
                        hidWeight[pos / sizeof(double)] = b.ReadDouble();
                        // Advance our position variable
                        pos += sizeof(double);
                    } // end while

                    // extend Ilary
                    Complex[] inputIlary = new Complex[Ilary.Length + 1];
                    // don't forget to set first element of Olary to 1
                    inputIlary[0] = new Complex(1, 0);
                    for (int i = 0; i < Ilary.Length; i++)
                        inputIlary[i + 1] = Ilary[i];
                    // end for

                    Olary[B] = Neuron(inputIlary, hidWeight);
                } // end for loop B
                for (int i = 0; i < hidLayerSize; i++)
                    Olary[i] /= Complex.Abs(Olary[i]);
                // end for loop
            } // end if hid
            else
                Olary = Ilary;
            // end else
            #endregion
            // ----------------- End of Hidden layer

            // ----------------- Building Output layer
            #region Output Layer
            Complex[] inputOlary = new Complex[Olary.Length + 1];
            // don't forget to set first element of Olary to 1
            inputOlary[0] = new Complex(1, 0);
            for (int i = 0; i < Olary.Length; i++)
                inputOlary[i + 1] = Olary[i];
            // end for
            //
            int newWeight1 = (Olary.Length + 1) * 2;
            double[] Rweight = new double[newWeight1];
            pos = 0;
            while (pos < newWeight1 * sizeof(double))
            {
                // Read double
                Rweight[pos / sizeof(double)] = b.ReadDouble();
                // Advance our position variable
                pos += sizeof(double);
            } // end while
            #endregion
            // ----------------- Apply Activation Function
            #region Activation Function
            // weighted sum
            Complex z = Neuron(inputOlary, Rweight);
            // get the angle
            double output;
            output = Math.Atan2(z.Imaginary, z.Real);
            // 
            if (output < 0)
                output = 2 * Math.PI + output;
            // end if
            double bb = (2 * Math.PI) / numberofsectors;
            // round
            output = Math.Truncate(output / bb);

            if (output > 255)
                if (output < 320)
                    output = 255;
                else
                    output = 0;
            // end if 

            #endregion
            // convert results to byte
            byte result = Convert.ToByte(output);
            // close file stream
            b.Close();
            return result;
        }

        /* -------------------------- Denoise by Patch ----------------------------------------------------------------- */

        public async Task<byte[,]> fdenoiseNeural(byte[,] noisyIm, int step, string fileName, int layer, int[] networkSize, int[] inputsPerSample, int numberofsectors, CancellationToken cancelToken, PauseToken pauseToken)
        {
            /*
                noisyIm: an image corrupted by AWG noise
                    the sliding window stride of the denoising 
                    process (a smaller stride will usually provide better results).
                The pixels of the clean image are assumed to be approximately in 
                the range 0..255.
            */

            #region Initialization

            window.SetText2("Initializing components...\r\n" + Environment.NewLine);
            window.SetProgress1(2);

            window.SetText2("Loading weights... ");
            // load the weights
            Complex[][,] weights = loadMlmvnWeights(fileName, layer, networkSize, inputsPerSample);
            window.SetText2("Done." + Environment.NewLine);

            window.SetText2("Configuring patch size... ");

            // size of input / output patch
            int patchSz = (int)Math.Sqrt(weights[0].GetLength(1) - 1); // <-- Implement outside of function to determine type of weights
            int patchSzOut = (int)Math.Sqrt(weights[layer - 1].GetLength(0));
            // Size of each sector on unit circle
            window.SetText2("Done.\r\n" + Environment.NewLine);

            window.SetText2("Input patch size is: " + patchSz + Environment.NewLine);
            window.SetText2("Output patch size is: " + patchSzOut + Environment.NewLine);

            // calculate the difference of the patches
            int p_diff = (patchSz - patchSzOut) / 2;
            // check if input is larger than output. If so, extend the image
            int height = noisyIm.GetLength(0);
            int origHeight = height;
            int width = noisyIm.GetLength(1);
            int origWidth = width;
            if (p_diff > 0)
            {
                noisyIm = new byte[height + p_diff * 2, width + p_diff * 2];
                noisyIm = functions.MirrorImage(noisyIm, height, width, p_diff);
                // if extended the image, update the size
                height = noisyIm.GetLength(0);
                width = noisyIm.GetLength(1);
            }

            #region Patch range configuration
            int pos = 0;
            // create arrays that contain the index ranges for row and column
            int[] range_y = new int[(height - patchSz) / step + 2];
            int[] range_x = new int[(width - patchSz) / step + 2];
            for (int i = 0; i < height - patchSz; i = i + step)
            {
                range_y[pos] = i;
                pos++;
            }
            // end for
            pos = 0;
            for (int i = 0; i < width - patchSz; i = i + step)
            {
                range_x[pos] = i;
                pos++;
            }
            // end for
            if (range_y[range_y.Length - 2] != height - patchSz)
            {
                range_y[range_y.Length - 1] = height - patchSz;
            }
            else
                Array.Resize(ref range_y, range_y.GetLength(0) - 1);
            // end if
            if (range_x[range_x.Length - 2] != height - patchSz)
            {
                range_x[range_x.Length - 1] = height - patchSz;
            }
            else
                Array.Resize(ref range_x, range_x.GetLength(0) - 1);
            // end if
            #endregion

            window.SetText2("\r\nDifference of the patche size is: " + p_diff + Environment.NewLine);
            window.SetText2("Beginning variable initialization... ");

            // pre-instantiate complex 2d-arrays
            // patch of interest
            int[,] cleanIm = new int[origHeight, origWidth];
            byte[,] counter = new byte[origHeight, origWidth]; // counts the overlapped patch, then later store the processed image.
            double[,] inputArray = new double[patchSz, patchSz];
            Complex[,] CinputArray = new Complex[patchSz, patchSz];
            // output patch to be stored to actual image
            byte[,] outputArray = new byte[patchSz, patchSz];
            byte[] output = new byte[(int)Math.Pow(patchSz, 2)];
            // used when patch needs to be transformed to 1d array
            Complex[] S = new Complex[inputArray.Length];
            // store outputs of network
            Complex[][] outputNeurons = new Complex[layer][];
            double[] dOutputNeurons = new double[networkSize[layer - 1]];
            // instanciate a jagged array to store outputs
            for (int i = 0; i < layer; i++)
                outputNeurons[i] = new Complex[networkSize[i]];
            // end for
            Complex sum = new Complex(0, 0);
            S[0] = new Complex(1, 0);
            // instantiate imaginary unit
            Complex complex1 = new Complex(0.0, 1.0);
            // processIndex as in old code
            int offset = ((patchSzOut - 3) / 2) + 1;
            double bb = (2 * Math.PI) / numberofsectors;

            window.SetText2("Done.\r\n" + Environment.NewLine);
            window.SetText2("Beginning the processing... \r\n" + Environment.NewLine);
            #endregion

            //int increment = 0;
            //double test2 = range_x.GetLength(0)/2;
            //double test3 = Math.Floor(test2);
            // --------------- Processing Begins ------------------------------
            // process each samples
            for (int row = 0; row < range_y.GetLength(0); row++) // for each row
            {
                for (int col = 0; col < range_x.GetLength(0); col++) // for each column
                {

                    #region process first layer
                    // process first layer
                    int ii = 0;
                    byte[,] src = Functions.CreatePatch(noisyIm, range_y[row], range_x[col], patchSz);
                    // upcast to double
                    Array.Copy(src, inputArray, src.Length);
                    // transformation of inputs into complex plane
                    for (int i = 0; i < patchSz; i++)
                        for (int j = 0; j < patchSz; j++)
                            CinputArray[i, j] = Exp(complex1 * 2 * Math.PI * inputArray[i, j] / numberofsectors);
                    // end nested for loop
                    // transform to 1d array
                    for (int i = 0; i < patchSz; i++)
                        for (int j = 0; j < patchSz; j++)
                            S[i * patchSz + j] = CinputArray[i, j];
                    // end for loop
                    #endregion
                    #region calculate weighted sum of first layer and its activation
                    // calculate weighted sum & activation
                    for (int i = 0; i < networkSize[0]; i++)
                    {
                        for (int j = 1; j < inputsPerSample[0]; j++)
                        {
                            sum = sum + weights[ii][i, j] * S[j - 1];
                        }
                        sum = sum + weights[ii][i, 0];
                        outputNeurons[ii][i] = sum;
                        sum = new Complex(0, 0);
                    } // end for

                    // apply continuous activation
                    for (int t = 0; t < networkSize[ii]; t++)
                        outputNeurons[ii][t] /= Complex.Abs(outputNeurons[ii][t]);
                    // end for
                    #endregion
                    #region calculate weighted sum of second to last layer
                    // ----------------- Process second to last hidden layers, then output layer
                    for (ii = 1; ii < layer - 1; ii++)
                    {
                        for (int i = 0; i < networkSize[ii]; i++)
                        {
                            for (int j = 1; j < inputsPerSample[ii]; j++)
                            {
                                sum = sum + weights[ii][i, j] * outputNeurons[ii - 1][j - 1];
                            }
                            sum = sum + weights[ii][i, 0];
                            outputNeurons[ii][i] = sum;
                            sum = new Complex(0, 0);
                        } // end for
                        // apply contiunous activation
                        for (int t = 0; t < networkSize[ii]; t++)
                            outputNeurons[ii][t] /= Complex.Abs(outputNeurons[ii][t]);
                        // end for
                    } // end for ii


                    // output layer
                    ii = layer - 1; // set to last layer
                    // calculate the weighted sum
                    for (int i = 0; i < networkSize[ii]; i++)
                    {
                        for (int j = 1; j < inputsPerSample[ii]; j++)
                        {
                            sum = sum + weights[ii][i, j] * outputNeurons[ii - 1][j - 1];
                        }
                        sum = sum + weights[ii][i, 0];
                        outputNeurons[ii][i] = sum;
                        sum = new Complex(0, 0);
                    } // end for

                    for (int jj = 0; jj < networkSize[ii]; jj++)
                    {
                        // calculate discrete output
                        // get angle
                        dOutputNeurons[jj] = Math.Atan2(outputNeurons[ii][jj].Imaginary, outputNeurons[ii][jj].Real);
                        if (dOutputNeurons[jj] < 0)
                            dOutputNeurons[jj] = 2 * Math.PI + dOutputNeurons[jj];
                        // end if
                        // round
                        dOutputNeurons[jj] = Math.Truncate(dOutputNeurons[jj] / bb);
                        //dOutputNeurons[jj] = Math.Floor(dOutputNeurons[jj]/bb);

                        if (dOutputNeurons[jj] > 255)
                            if (dOutputNeurons[jj] < 320)
                                dOutputNeurons[jj] = 255;
                            else
                                dOutputNeurons[jj] = 0;
                        // end if 
                        // convert results to byte
                        output[jj] = Convert.ToByte(dOutputNeurons[jj]);
                    } // end for

                    #endregion second to last layer
                    #region Process image
                    // resize
                    for (int i = 0; i < patchSzOut; i++)
                        for (int j = 0; j < patchSzOut; j++)
                            outputArray[i, j] = output[p_diff + j + (i * patchSz)];
                    // end for
                    // add to the actual image
                    for (int i = 0; i < patchSzOut; i++)
                        for (int j = 0; j < patchSzOut; j++)
                        {
                            //if (counter[range_y[row] + i, range_x[col] + j] == 0)
                            //{
                            cleanIm[range_y[row] + i, range_x[col] + j] += outputArray[i, j];
                            counter[range_y[row] + i, range_x[col] + j]++;
                            //}
                        }
                    // end for
                    // end for
                    #endregion
                    #region Form elements
                    if (cancelToken.IsCancellationRequested)
                        cancelToken.ThrowIfCancellationRequested();

                    // Action when pause button is clicked
                    await pauseToken.WaitWhilePausedAsync();

                    window.SetProgress1(1);
                    #endregion
                }//); // end col for loop

                window.SetText2("Patches in row " + (row + 1) + " of " + range_y.Length + " done." + Environment.NewLine);

            }//); // end row for loop
            #region Average
            // Average
            for (int row = 0; row < origHeight; row++) // for each row
            {
                for (int col = 0; col < origWidth; col++) // for each column
                {
                    cleanIm[row, col] /= counter[row, col];
                    counter[row, col] = Convert.ToByte(cleanIm[row, col]);
                }
            }
            #endregion

            #region Old code
            //double[,] pixel_weights = new double[patchSzOut, patchSzOut];
            //// median pixel ceiling(17/2) = 9
            //int mid = (int)Math.Ceiling((double)patchSzOut/2);
            //// floor(17/2) / 2 = 4
            //int sig = (int)Math.Floor((double)patchSzOut / 2) / weightSig;
            //// initialize pixel_weights
            //double d = 0;
            //for (int i = 0; i < patchSzOut; i++)
            //    for (int j = 0; j < patchSzOut; j++)
            //    {
            //        d = Math.Sqrt(Math.Pow((i - mid),2) + Math.Pow((j - mid), 2));
            //        pixel_weights[i, j] = Math.Exp(Math.Pow(-d,2) / (2 * Math.Pow(sig, 2))) / (sig * Math.Sqrt(2 * Math.PI));
            //    }
            //// end for
            //// obtain the max of pixel_weights and subtract each element with it
            //// to achieve mean zero
            //double max = pixel_weights.Cast<double>().Max();
            //for (int i = 0; i < patchSzOut; i++)
            //    for (int j = 0; j < patchSzOut; j++)
            //        pixel_weights[i, j] = pixel_weights[i, j] - max;
            //// end for

            //// Upcast byte to double
            //double[,] dnoisyIm = new double[noisyIm.GetLength(0), noisyIm.GetLength(1)];
            //Array.Copy(noisyIm, dnoisyIm, noisyIm.Length);
            //// subract 0.5 and multiply by 0.2 to achieve approximately mean zero and 
            //// variance close to one
            //for (int i = 0; i < dnoisyIm.GetLength(0); i++)
            //    for (int j = 0; j < dnoisyIm.GetLength(1); j++ )
            //        dnoisyIm[i, j] = (((dnoisyIm[i, j] / 255) - 0.5) / 0.2);
            //// end for

            //int chunkSize = 1000;
            //int pos = 0;
            //// create arrays that contain the index ranges for row and column
            //int[] range_y = new int[(dnoisyIm.GetLength(0) - patchSz)/step + 2];
            //int[] range_x = new int[(dnoisyIm.GetLength(1) - patchSz)/step + 2];
            //for (int i = 0; i < noisyIm.GetLength(0) - patchSz; i = i + step)
            //{
            //    range_y[pos] = i;
            //    pos++;
            //}
            //// end for
            //pos = 0;
            //for (int i = 0; i < noisyIm.GetLength(1) - patchSz; i = i + step)
            //{ 
            //    range_x[pos] = i;
            //    pos++;
            //}
            //// end for

            //// if the ranges do not include the last available row / column, include them
            //if (range_y[range_y.GetLength(0) - 2] != noisyIm.GetLength(0) - patchSz)
            //    range_y[range_y.GetLength(0) - 1] = noisyIm.GetLength(0) - patchSz;
            //else
            //    Array.Resize(ref range_y, range_y.GetLength(0) - 1);
            //// end if
            //if (range_x[range_x.GetLength(0) - 2] != noisyIm.GetLength(1) - patchSz)
            //    range_x[range_y.GetLength(0) - 1] = noisyIm.GetLength(1) - patchSz;
            //else
            //    Array.Resize(ref range_x, range_x.GetLength(0) - 1);
            //// end if

            //double[,] res = new double[(int)Math.Pow(patchSz, 2), chunkSize];
            //double[,] part = new double[(int)Math.Pow(patchSz, 2) + layer - 1, chunkSize];
            //int[,] positions_out = new int[2, chunkSize];
            //byte[,] denoisedIm = new byte[noisyIm.GetLength(0),noisyIm.GetLength(1)];
            //Complex[,] wIm = new Complex[noisyIm.GetLength(0), noisyIm.GetLength(1)];
            //byte[,] p = new byte[patchSz, patchSz];
            //byte[] p1 = new byte[(int)Math.Pow(patchSz,2)];

            // --------------------- Processing Image Begin ---------------------------------------
            //int idx = -1;
            //for (int y = 0; y < range_y.GetLength(0); y++)
            //{
            //    for (int x = 0; x < range_x.GetLength(0); x++)
            //    {
            //        // copy particular input patch from noisy image
            //        for (int i = 0; i < patchSz; i++)
            //            for (int j = 0; j < patchSz; j++)
            //                p[i, j] = noisyIm[range_y[y] + i, range_x[x] + j];
            //        // end for

            //        // increment index
            //        idx++;
            //        // convert p to 1d array
            //        Buffer.BlockCopy(p,0, p1, 0, p1.Length * sizeof(byte));
            //        // copy whole patch as a row to the indexed row of res;
            //        for (int i = 0; i < res.GetLength(0); i++)
            //            res[i, idx] = p1[i];
            //        // end for
            //        // copy the index of the particular iteration
            //        positions_out[0, idx] = y;
            //        positions_out[1, idx] = x;
            //        // every time idx reaches 1000, below executes. after that, idx is reset to 0
            //        // ------------------------ Prediction --------------------------------------
            //        if ( idx >= chunkSize - 1 )
            //        {
            //            // copy res to part
            //            Array.Copy(res, part, res.Length);
            //            for ( int i = 0; i<layer;i++)
            //            {
            //                for (int j = 0; j < part.GetLength(1);j++ )
            //                    part[part.GetLength(0) - layer + 1, j] = 1; 
            //                // end for
            //            } // end for
            //        } // end if
            //    }
            //}
            #endregion

            return counter;
        } // end method

        public async Task<byte[,]> fdenoiseNeural2(byte[,] noisyIm, int step, string fileName, int layer, int[] networkSize, int[] inputsPerSample, int numberofsectors, CancellationToken cancelToken, PauseToken pauseToken)//, double progressBar1, double progressBar1Max)
        {
            /*
            *   noisyIm: an image corrupted by AWG noise
                  * the sliding window stride of the denoising 
                    process (a smaller stride will usually provide better results).
                The pixels of the clean image are assumed to be approximately in 
                the range 0..255.
            */

            #region Initialization

            window.SetText2("Using the new patch method.\r\n" + Environment.NewLine);

            window.SetText2("Initializing Components...\r\n" + Environment.NewLine);
            window.SetProgress1(2);

            window.SetText2("Loading weights... ");
            // load the weights
            Complex[][,] weights = loadMlmvnWeights(fileName, layer, networkSize, inputsPerSample);
            window.SetText2("Done." + Environment.NewLine);

            window.SetText2("Configuring Patch Size... ");
            // size of input / output patch
            int patchSz = (int)Math.Sqrt(weights[0].GetLength(1));
            int patchSzOut = (int)Math.Sqrt(weights[layer - 1].GetLength(0));
            // Size of each sector on unit circle

            window.SetText2("Done.\r\n" + Environment.NewLine);

            window.SetText2("Input patch size is: " + patchSz + Environment.NewLine);
            window.SetText2("Output patch size is: " + patchSzOut + Environment.NewLine);

            // calculate the difference of the patches
            int p_diff = (patchSz - patchSzOut) / 2;
            // check if input is larger than output. If so, extend the image
            int height = noisyIm.GetLength(0);
            int origHeight = height;
            int width = noisyIm.GetLength(1);
            int origWidth = width;
            if (p_diff > 0)
            {
                noisyIm = new byte[height + p_diff * 2, width + p_diff * 2];
                noisyIm = functions.MirrorImage(noisyIm, height, width, p_diff);
                // if extended the image, update the size
                height = noisyIm.GetLength(0);
                width = noisyIm.GetLength(1);
            }

            #region Patch range configuration

            // interval determines how many pixels would be skipped before new patch will be placed.
            // For example, if step is 3 and patch size is 13, 3 * 2 = 6 pixels would be overlapped for non-edge patches.
            // Therefore, 13 - 6 = 7 pixels will be skipped.
            int interval = patchSz - (step * 2);

            // offsetX and offsetY determine the number of "leftover" pixels on the right and bottom edges.
            // For example, if 512 * 512 image will be filled by 13 * 13 patches, 512 - ((13-3) % interval
            int offsetX = (width - (patchSz - step)) % interval;
            int offsetY = (height - (patchSz - step)) % interval;
            // reserve the array to indicate the index of patches. include one position for the fist patch.  And reserve extra one position just in case 
            // we need to fill the offset
            int[] range_x = new int[(width - (patchSz - step)) / interval + 2];
            int[] range_y = new int[(height - (patchSz - step)) / interval + 2];
            int pos = 0;
            // fill the arrays with intervals.  ignore the last element because we don't know if it's necessary yet
            for (int i = 0; i < range_x.GetLength(0) - 1; i++)
            {
                range_x[i] = pos;
                pos += interval;
            }
            pos = 0;
            for (int i = 0; i < range_y.GetLength(0) - 1; i++)
            {
                range_y[i] = pos;
                pos += interval;
            }
            // end for

            // correct last index if necessary
            // if offsetX and Y are equal to 0, that means no fitting is necessary.  Therefore, just resize the array to have
            // one less length.  Else, fill the last element of the array with the index according to the offsets
            if (offsetX == 0)
                Array.Resize(ref range_x, range_x.GetLength(0) - 1);
            else
                range_x[range_x.GetLength(0) - 1] = width - patchSz;
            // end if
            if (offsetY == 0)
                Array.Resize(ref range_y, range_y.GetLength(0) - 1);
            else
                range_y[range_y.GetLength(0) - 1] = height - patchSz;
            // end if
            #endregion

            window.SetText2("\r\nDifference of the patche size is: " + p_diff + Environment.NewLine);
            window.SetText2("Beginning variable initialization... ");

            // pre-instantiate complex 2d-arrays
            // patch of interest
            byte[,] cleanIm = new byte[origHeight, origWidth];
            //byte[,] counter = new byte[origHeight, origWidth]; // counts the overlapped patch, then later store the processed image.
            double[,] inputArray = new double[patchSz, patchSz];
            Complex[,] CinputArray = new Complex[patchSz, patchSz];
            // output patch to be stored to actual image
            byte[,] outputArray = new byte[patchSz, patchSz];
            byte[] output = new byte[(int)Math.Pow(patchSz, 2)];
            // used when patch needs to be transformed to 1d array
            Complex[] S = new Complex[inputArray.Length];
            // store outputs of network
            Complex[][] outputNeurons = new Complex[layer][];
            double[] dOutputNeurons = new double[networkSize[layer - 1]];
            // instanciate a jagged array to store outputs
            for (int i = 0; i < layer; i++)
                outputNeurons[i] = new Complex[networkSize[i]];
            // end for
            Complex sum = new Complex(0, 0);
            S[0] = new Complex(1, 0);
            // instantiate imaginary unit
            Complex complex1 = new Complex(0.0, 1.0);
            // processIndex as in old code
            int offset = ((patchSzOut - 3) / 2) + 1;
            double bb = (2 * Math.PI) / numberofsectors;

            window.SetText2("Done.\r\n" + Environment.NewLine);
            window.SetText2("Beginning Processing... \r\n" + Environment.NewLine);
            #endregion

            // --------------- Processing Begins ------------------------------
            // process each samples
            for (int row = 0; row < range_y.GetLength(0); row++) // for each row
            {
                for (int col = 0; col < range_x.GetLength(0); col++) // for each column
                {

                    #region process first layer
                    // process first layer
                    int ii = 0;
                    byte[,] src = Functions.CreatePatch(noisyIm, range_y[row], range_x[col], patchSz);
                    // upcast to double
                    Array.Copy(src, inputArray, src.Length);
                    // transformation of inputs into complex plane
                    for (int i = 0; i < patchSz; i++)
                        for (int j = 0; j < patchSz; j++)
                            CinputArray[i, j] = Exp(complex1 * 2 * Math.PI * inputArray[i, j] / numberofsectors);
                    // end nested for loop
                    // transform to 1d array
                    for (int i = 0; i < patchSz; i++)
                        for (int j = 0; j < patchSz; j++)
                            S[i * patchSz + j] = CinputArray[i, j];
                    // end for loop
                    #endregion
                    #region calculate weighted sum of first layer and its activation
                    // calculate weighted sum & activation
                    for (int i = 0; i < networkSize[0]; i++)
                    {
                        for (int j = 1; j < inputsPerSample[0]; j++)
                        {
                            sum = sum + weights[ii][i, j] * S[j - 1];
                        }
                        sum = sum + weights[ii][i, 0];
                        outputNeurons[ii][i] = sum;
                        sum = new Complex(0, 0);
                    } // end for

                    // apply continuous activation
                    for (int t = 0; t < networkSize[ii]; t++)
                        outputNeurons[ii][t] /= Complex.Abs(outputNeurons[ii][t]);
                    // end for
                    #endregion
                    #region calculate weighted sum of second to last layer
                    // ----------------- Process second to last hidden layers, then output layer
                    for (ii = 1; ii < layer - 1; ii++)
                    {
                        for (int i = 0; i < networkSize[ii]; i++)
                        {
                            for (int j = 1; j < inputsPerSample[ii]; j++)
                            {
                                sum = sum + weights[ii][i, j] * outputNeurons[ii - 1][j - 1];
                            }
                            sum = sum + weights[ii][i, 0];
                            outputNeurons[ii][i] = sum;
                            sum = new Complex(0, 0);
                        } // end for
                        // apply contiunous activation
                        for (int t = 0; t < networkSize[ii]; t++)
                            outputNeurons[ii][t] /= Complex.Abs(outputNeurons[ii][t]);
                        // end for
                    } // end for ii


                    // output layer
                    ii = layer - 1; // set to last layer
                    // calculate the weighted sum
                    for (int i = 0; i < networkSize[ii]; i++)
                    {
                        for (int j = 1; j < inputsPerSample[ii]; j++)
                        {
                            sum = sum + weights[ii][i, j] * outputNeurons[ii - 1][j - 1];
                        }
                        sum = sum + weights[ii][i, 0];
                        outputNeurons[ii][i] = sum;
                        sum = new Complex(0, 0);
                    } // end for

                    for (int jj = 0; jj < networkSize[ii]; jj++)
                    {
                        // calculate discrete output
                        // get angle
                        dOutputNeurons[jj] = Math.Atan2(outputNeurons[ii][jj].Imaginary, outputNeurons[ii][jj].Real);
                        if (dOutputNeurons[jj] < 0)
                            dOutputNeurons[jj] = 2 * Math.PI + dOutputNeurons[jj];
                        // end if
                        // round
                        dOutputNeurons[jj] = Math.Truncate(dOutputNeurons[jj] / bb);
                        //dOutputNeurons[jj] = Math.Floor(dOutputNeurons[jj]/bb);

                        if (dOutputNeurons[jj] > 255)
                            if (dOutputNeurons[jj] < 320)
                                dOutputNeurons[jj] = 255;
                            else
                                dOutputNeurons[jj] = 0;
                        // end if 
                        // convert results to byte
                        output[jj] = Convert.ToByte(dOutputNeurons[jj]);
                    } // end for

                    #endregion second to last layer
                    #region Process image
                    // resize
                    for (int i = 0; i < patchSzOut; i++)
                        for (int j = 0; j < patchSzOut; j++)
                            outputArray[i, j] = output[p_diff + j + (i * patchSz)];
                    // end for

                    // Output to an acutal image.
                    /* Codes below outputs the calculated patch into the output image.  Although the size of patch is 13 x 13, only center 7 * 7 will be copied to the
                     * image because of overlapping method, with the only exception of when the patch touches the edge of the image.  In order to determine this, we will
                     * first check whether the coordinate of the patch is either 0 or width(height) - patchSz. if so, we need to take the borders into account.  Otherwise,
                     * we just need to copy center 7 x 7 pixels onto corresponding coordinates.
                     */

                    // check
                    if (range_y[row] == 0 || range_y[row] == height - patchSz || range_x[col] == 0 || range_x[col] == width - patchSz)
                    {
                        // startY and startX determines the starting coordinate of the local patch; it's initialized with step. if the patch is touching the edge, we need to
                        // set them to 0, so whole patch side will be copied. The same for endY and endX.
                        int startY, startX;
                        startY = startX = step;
                        int endY, endX;
                        endY = endX = patchSzOut - step;
                        // All outer patches
                        if (range_y[row] == 0)
                            startY = 0;
                        // end if
                        if (range_y[row] == height - patchSz)
                        {
                            // startY = patchSzOut - (height - (range_y[row - 1] + patchSz - (step * 2)));
                            startY = patchSzOut - offsetY;
                            endY = patchSzOut;
                        }
                        // end if

                        if (range_x[col] == 0)
                            startX = 0;
                        // end if
                        if (range_x[col] == width - patchSz)
                        {
                            startX = patchSzOut - offsetX;
                            endX = patchSzOut;
                        }
                        // end if

                        // Place patches
                        for (int i = startY; i < endY; i++)
                        {
                            for (int j = startX; j < endX; j++)
                            {
                                cleanIm[range_y[row] + i, range_x[col] + j] = outputArray[i, j];
                            } // end for
                        } //end
                    }
                    else
                    {
                        // All Inner Patches have outer edges cut off
                        for (int i = step; i < patchSzOut - step; i++)
                        {
                            for (int j = step; j < patchSzOut - step; j++)
                            {
                                cleanIm[range_y[row] + i, range_x[col] + j] = outputArray[i, j];
                            } // end for
                        } //end
                    }

                    #endregion
                    #region Form elements
                    if (cancelToken.IsCancellationRequested)
                        cancelToken.ThrowIfCancellationRequested();

                    // Action when pause button is clicked
                    await pauseToken.WaitWhilePausedAsync();

                    window.SetProgress1(1);
                    #endregion
                } // end col for loop               

                window.SetText2("Patches in row " + (row + 1) + " of " + range_y.Length + " done." + Environment.NewLine);
            } // end row for loop

            return cleanIm;
        } // end method

        /* -------------------------- Useful Functions ------------------------------------------------------------------ */

        // exponential of complex numbers
        public static Complex Exp(Complex Exponent)
        {
            Complex X, P, Frac, L;
            double I;
            X = Exponent;
            Frac = X;
            P = (1.0 + X);
            I = 1.0;

            do
            {
                I++;
                Frac *= (X / I);
                L = P;
                P += Frac;
            } while (L != P);

            return P;
        }

        // load weights in matrix from binary file
        public static Complex[][,] loadMlmvnWeights(string fileName, int layer, int[] networkSize, int[] inputsPerSample)
        {
            /* fileName - file name of the weight file
             * layer - number of layers
             * networkSize - size of network
             * inputsPerSample - number of inputs per sample
             */

            // create jagged array to store weights
            Complex[][,] network = new Complex[layer][,];
            for (int i = 0; i < layer; i++)
                network[i] = new Complex[networkSize[i], inputsPerSample[i]];
            // end if
            // initialize the network to zeros
            for (int ii = 0; ii < layer; ii++)
            {
                for (int i = 0; i < network[ii].GetLength(0); i++)
                {
                    for (int j = 0; j < network[ii].GetLength(1); j++)
                        network[ii][i, j] = new Complex(0, 0);
                } // end for
            } // end for

            // Load the weights
            BinaryReader b = new BinaryReader(File.Open(fileName, FileMode.Open));
            int pos = 0;
            int ind = 0;
            for (int ii = 0; ii < layer; ii++)
            {
                int len2 = network[ii].GetLength(0);
                for (int jj = 0; jj < len2; jj++)
                {
                    int nw = 0;
                    if (ii == 0)
                        nw = (inputsPerSample[ii]) * 2;
                    else
                        nw = (networkSize[ii - 1] + 1) * 2;
                    // end if
                    double[] aa = new double[nw];
                    if (b.BaseStream.Position != b.BaseStream.Length)
                    {
                        pos = 0;
                        while (pos < nw * sizeof(double))
                        {
                            // Read double
                            aa[pos / sizeof(double)] = b.ReadDouble();
                            // Advance our position variable
                            pos += sizeof(double);
                        } // end while
                        ind = 0;
                        for (int kk = 0; kk < aa.Length; kk += 2)
                        {
                            network[ii][jj, ind] = new Complex(aa[kk], aa[kk + 1]);
                            ind++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n\n eof found before the end of network\n\n");
                    } // end if                   
                } // end for jj
            } // end for ii
            b.Dispose();
            return network;
        } // end method

        public static void saveMlmvnWeights(string fileName, Complex[][,] network, int[] networkSize)
        {
            int layer = network.Length;

            int[] inputsPerSample = new int[layer];
            inputsPerSample[0] = networkSize[layer - 1] + 1;
            for (int i = 1; i < layer; i++)
                inputsPerSample[i] = networkSize[0] + 1;
            // end for

            // create BinaryWriter object to write weights to file
            using (BinaryWriter b = new BinaryWriter(File.OpenWrite(fileName)))
            {
                // write file.
                // ii - layer
                // jj - neuron
                // kk - each element of the neuron
                for (int ii = 0; ii < layer; ii++)
                {
                    for (int jj = 0; jj < networkSize[ii]; jj++)
                    {
                        for (int kk = 0; kk < inputsPerSample[ii]; kk++)
                        {
                            b.Write((double)network[ii][jj, kk].Real);
                            b.Write((double)network[ii][jj, kk].Imaginary);
                        }
                    }
                }
                Console.WriteLine("Saved Successfully!");
                // dispose binary writer
                b.Dispose();
            }
        }

        public static Complex Neuron(Complex[] input, double[] weights)
        {
            Complex[] x = new Complex[weights.Length / 2];
            int Xcounter = 0;

            for (int i = 0; i < weights.Length; i += 2)
            {
                x[Xcounter] = new Complex(weights[i], weights[i + 1]);
                Xcounter++;
            }
            Complex sum = new Complex(0, 0);
            // get the weighted sum
            for (int i = 0; i < weights.Length / 2; i++)
            {
                x[i] *= input[i];
                sum += x[i];
            }
            return sum;
        } // end method Neuron

        public static Complex Neuron(Complex[] input, Complex[] weights)
        {
            Complex sum = new Complex(0, 0);
            // get the weighted sum
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] *= input[i];
                sum += weights[i];
            }
            return sum;
        } // end method Neuron

        public int[,] TEST(string fileNameSamples, int numberOfInputSamples, string fileNameWeights, int layer, int[] networkSize, int[] inputsPerSample, int numberofsectors)
        {
            #region Initialization
            int twoInputsPerSample = networkSize[layer - 1] * 2;
            //Window.SetText1("\r\nInitializing components... ");
            // load the samples
            byte[,] samples = loadLearningSamples(fileNameSamples, numberOfInputSamples, twoInputsPerSample);
            window.SetText1("\r\nInitializing components... Done." + Environment.NewLine);
            //Window.SetText1("Loading weights... ");
            // load the weights
            Complex[][,] weights = loadMlmvnWeights(fileNameWeights, layer, networkSize, inputsPerSample);
            window.SetText1("Loading weights... Done." + Environment.NewLine);
            //Window.SetText1("Loading learning samples... ");

            double twoPi = Math.PI * 2;
            double sectorSize = twoPi / numberofsectors;
            int numberOfOutputs = networkSize[layer - 1];
            int rowInputs = samples.GetLength(0);
            int colInputs = samples.GetLength(1) / 2;
            // Desired outputs
            byte[,] desiredOutputs = new byte[rowInputs, colInputs];
            for (int i = 0; i < rowInputs; i++)
                for (int j = 0; j < colInputs; j++)
                    desiredOutputs[i, j] = samples[i, j + colInputs];
            // end for loops

            // Resized Inputs
            byte[,] inputs = new byte[rowInputs, colInputs];
            for (int i = 0; i < rowInputs; i++)
                for (int j = 0; j < colInputs; j++)
                    inputs[i, j] = samples[i, j];
            // end for loops
            Complex[,] Cinputs = new Complex[rowInputs, colInputs];
            int[,] networkOutputs = new int[rowInputs, colInputs];
            //for (int i = 0; i < rowInputs; i++)
            //    for (int j = 0; j < colInputs; j++)
            //        networkOutputs[i, j] = new Complex(0, 0);
            // end
            double[] networkErrors = new double[rowInputs];
            Complex[][] neuronOutputs = new Complex[layer][];
            // instanciate a jagged array to store outputs
            for (int i = 0; i < layer; i++)
                neuronOutputs[i] = new Complex[networkSize[i]];
            // end for

            double[] dNeuronOutputs = new double[networkSize[layer - 1]];

            Complex sum = new Complex(0, 0);
            Complex complex1 = new Complex(0.0, 1.0);

            // transformation of inputs into complex plane
            for (int i = 0; i < rowInputs; i++)
                for (int j = 0; j < colInputs; j++)
                    Cinputs[i, j] = Exp(complex1 * 2 * Math.PI * inputs[i, j] / numberofsectors);
            // end nested for loop

            window.SetText1("Done." + Environment.NewLine + "Beginning the processing... ");
            #endregion

            // --------------- BEGIN OUTPUT CALCULATION ------------------------------
            // process each samples
            for (int aa = 0; aa < numberOfInputSamples; aa++) // for each row
            {
                #region calculate weighted sum of first layer and its activation
                // process first layer
                int ii = 0;

                // calculate weighted sum & activation
                for (int i = 0; i < networkSize[0]; i++)
                {
                    for (int j = 1; j < inputsPerSample[0]; j++)
                    {
                        sum = sum + weights[ii][i, j] * Cinputs[aa, j - 1];
                    }
                    sum = sum + weights[ii][i, 0];
                    neuronOutputs[ii][i] = sum;
                    sum = new Complex(0, 0);
                } // end for

                // apply continuous activation
                for (int t = 0; t < networkSize[ii]; t++)
                    neuronOutputs[ii][t] /= Complex.Abs(neuronOutputs[ii][t]);
                // end for
                #endregion
                #region calculate weighted sum of second to last layer
                // ----------------- Process second to last hidden layers, then output layer
                for (ii = 1; ii < layer - 1; ii++)
                {
                    for (int i = 0; i < networkSize[ii]; i++)
                    {
                        for (int j = 1; j < inputsPerSample[ii]; j++)
                        {
                            sum = sum + weights[ii][i, j] * neuronOutputs[ii - 1][j - 1];
                        }
                        sum = sum + weights[ii][i, 0];
                        neuronOutputs[ii][i] = sum;
                        sum = new Complex(0, 0);
                    } // end for
                    // apply contiunous activation
                    for (int t = 0; t < networkSize[ii]; t++)
                        neuronOutputs[ii][t] /= Complex.Abs(neuronOutputs[ii][t]);
                    // end for
                } // end for ii


                // output layer
                ii = layer - 1; // set to last layer
                // calculate the weighted sum
                for (int i = 0; i < networkSize[ii]; i++)
                {
                    for (int j = 1; j < inputsPerSample[ii]; j++)
                    {
                        sum = sum + weights[ii][i, j] * neuronOutputs[ii - 1][j - 1];
                    }
                    sum = sum + weights[ii][i, 0];
                    neuronOutputs[ii][i] = sum;
                    sum = new Complex(0, 0);
                } // end for

                for (int jj = 0; jj < networkSize[ii]; jj++)
                {
                    // calculate discrete output
                    // get angle
                    dNeuronOutputs[jj] = Math.Atan2(neuronOutputs[ii][jj].Imaginary, neuronOutputs[ii][jj].Real);
                    // if output is less than 0, add 2 pi to make it positive
                    if (dNeuronOutputs[jj] < 0)
                        dNeuronOutputs[jj] = 2 * Math.PI + dNeuronOutputs[jj];
                    // end if
                    // round
                    dNeuronOutputs[jj] = Math.Truncate(dNeuronOutputs[jj] / sectorSize);
                    //dOutputNeurons[jj] = Math.Floor(dOutputNeurons[jj]/bb);
                    // convert results to byte... did not work correctly, because it could be more than 255.  So let it be integer.
                    networkOutputs[aa, jj] = Convert.ToInt32(dNeuronOutputs[jj]);
                } // end for
                #endregion second to last layer
            } // end row for loop
            window.SetText1("Done." + Environment.NewLine + "Calculating errors... " + Environment.NewLine);
            // -------------- END OUTPUT CALCULATION -----------------------------
            double mse = 0;
            double rmse = 0;
            // -------------- BEGIN NET ERROR CALCULATION ------------------------
            for (int aa = 0; aa < numberOfInputSamples; aa++)
            {
                for (int i = 0; i < colInputs; i++)
                {
                    networkErrors[aa] += Math.Pow((desiredOutputs[aa, i] - networkOutputs[aa, i]), 2);
                }
                networkErrors[aa] /= numberOfOutputs;
                mse += networkErrors[aa];
            } // end for aa

            // calculate mse
            mse /= numberOfInputSamples;
            // calculate rmse
            rmse = Math.Sqrt(mse);
            window.SetText1("RMSE: " + rmse + Environment.NewLine + Environment.NewLine);

            return networkOutputs;
        }

        public async Task<Complex[][,]> Learning(string fileNameSamples, int numberOfInputSamples, string fileNameWeights, int layer, int[] networkSize, int[] inputsPerSample, int numberofsectors, double globalThreasholdValue, double localThresholdValue, bool randomWeights, CancellationToken cancelToken, PauseToken pauseToken)
        {
            #region Initialization
            int twoInputsPerSample = networkSize[layer - 1] * 2;

            window.SetText1("Initializing components..." + Environment.NewLine);
            // load the samples
            window.SetText1("Loading learning samples... ");
            byte[,] samples = loadLearningSamples(fileNameSamples, numberOfInputSamples, twoInputsPerSample);
            window.SetText1("Done." + Environment.NewLine);
            // Initial Weights Initialization
            #region Weights Initialization
            Random random = new Random();
            double real;
            double imag;
            Complex[][,] weights = new Complex[layer][,];
            if (randomWeights)
            {
                // generate random weights
                // initialize weights matrix       
                for (int ii = 0; ii < layer; ii++)
                {
                    weights[ii] = new Complex[networkSize[ii], inputsPerSample[ii]];
                    //if (ii == 0)
                    //    weights[ii] = new Complex[networkSize[ii], inputsPerSample[ii]];
                    //else
                    //    weights[ii] = new Complex[networkSize[ii], networkSize[ii - 1]];
                    // now generate random numbers
                    for (int i = 0; i < weights[ii].GetLength(0); i++)
                        for (int j = 0; j < weights[ii].GetLength(1); j++)
                        {
                            real = random.NextDouble() - 0.5;
                            imag = random.NextDouble() - 0.5;
                            weights[ii][i, j] = new Complex(real, imag);
                        } // end for j
                    // end for i
                } // end for ii
            }
            else
            {
                window.SetText1("Loading weights... ");
                // load the weights
                weights = loadMlmvnWeights(fileNameWeights, layer, networkSize, inputsPerSample);
                window.SetText1("Done.\n" + Environment.NewLine);
            } // end if 
            #endregion
            double twoPi = Math.PI * 2;
            Complex complex1 = new Complex(0.0, 1.0);
            double sectorSize = twoPi / numberofsectors;
            Complex[] Sector = new Complex[numberofsectors];
            for (int i = 0; i < numberofsectors; i++)
            {
                double angSector = twoPi * i / numberofsectors;
                Sector[i] = Complex.Exp(complex1 * angSector);
            }
            int numberOfOutputs = networkSize[layer - 1];
            int rowInputs = samples.GetLength(0);
            int colInputs = samples.GetLength(1) / 2;
            // Desired outputs
            byte[,] desiredOutputs = new byte[rowInputs, colInputs];
            for (int i = 0; i < rowInputs; i++)
                for (int j = 0; j < colInputs; j++)
                    desiredOutputs[i, j] = samples[i, j + colInputs];
            // end for loops

            // Resized Inputs
            byte[,] inputs = new byte[rowInputs, colInputs];
            for (int i = 0; i < rowInputs; i++)
                for (int j = 0; j < colInputs; j++)
                    inputs[i, j] = samples[i, j];
            // end for loops
            Complex[,] Cinputs = new Complex[rowInputs, colInputs];
            int[,] networkOutputs = new int[rowInputs, colInputs];
            //for (int i = 0; i < rowInputs; i++)
            //    for (int j = 0; j < colInputs; j++)
            //        networkOutputs[i, j] = new Complex(0, 0);
            // end
            double[] networkErrors = new double[rowInputs];
            Complex[][] neuronOutputs = new Complex[layer][];
            // instanciate a jagged array to store outputs
            for (int i = 0; i < layer; i++)
                neuronOutputs[i] = new Complex[networkSize[i]];
            // end for
            double[] dNeuronOutputs = new double[networkSize[layer - 1]];
            Complex[][] neuronErrors = neuronOutputs;
            Complex[][] weightedSum = new Complex[layer][];
            // instanciate a jagged array to store outputs
            for (int i = 0; i < layer; i++)
                weightedSum[i] = new Complex[networkSize[i]];
            // end for
            Complex sum = new Complex(0, 0);
            // transformation of inputs into complex plane
            for (int i = 0; i < rowInputs; i++)
                for (int j = 0; j < colInputs; j++)
                    Cinputs[i, j] = Exp(complex1 * 2 * Math.PI * inputs[i, j] / numberofsectors);
            // end nested for loop
            // initialize error criteria
            double mse = 0;
            double rmse = 0;
            double[] learningRate;
            // check if learning is finished
            bool finishedLearning = false;
            // counts each cycle
            int iteration = 0;

            // output calculation
            Complex[,] temp;
            Complex[,] a1;
            Complex[] b1;
            Complex[] c1;
            Complex d1;
            Complex[,] e1;
            Complex[,] f1;

            window.SetText1("Beginning the learning of the weights...\r\n" + Environment.NewLine);
            #endregion

            #region RMSE ALGORITHM
            // repeats process until learning converges
            while (!finishedLearning)
            {
                // increment iteration
                iteration++;
                // --------------- BEGIN OUTPUT CALCULATION ------------------------------
                #region OUTPUT CALCULATION
                // process each samples
                for (int aa = 0; aa < numberOfInputSamples; aa++) // for each row
                {
                    #region calculate weighted sum of first layer and its activation
                    // process first layer
                    int ii = 0;

                    // calculate weighted sum & activation
                    for (int i = 0; i < networkSize[0]; i++)
                    {
                        for (int j = 1; j < inputsPerSample[0]; j++)
                        {
                            sum = sum + weights[ii][i, j] * Cinputs[aa, j - 1];
                        }
                        sum = sum + weights[ii][i, 0];
                        neuronOutputs[ii][i] = sum;
                        //weightedSum[ii][i] = sum;
                        sum = new Complex(0, 0);
                    } // end for

                    // apply continuous activation
                    for (int t = 0; t < networkSize[ii]; t++)
                        neuronOutputs[ii][t] /= Complex.Abs(neuronOutputs[ii][t]);
                    // end for
                    #endregion
                    #region calculate weighted sum of second to last layer
                    // ----------------- Process second to last hidden layers ---------------
                    for (ii = 1; ii < layer - 1; ii++)
                    {
                        for (int i = 0; i < networkSize[ii]; i++)
                        {
                            for (int j = 1; j < inputsPerSample[ii]; j++)
                            {
                                sum = sum + weights[ii][i, j] * neuronOutputs[ii - 1][j - 1];
                            }
                            sum = sum + weights[ii][i, 0];
                            neuronOutputs[ii][i] = sum;
                            //weightedSum[ii][i] = sum;
                            sum = new Complex(0, 0);
                        } // end for
                        // apply contiunous activation
                        for (int t = 0; t < networkSize[ii]; t++)
                            neuronOutputs[ii][t] /= Complex.Abs(neuronOutputs[ii][t]);
                        // end for
                    } // end for ii


                    // ----------------- Process output layer --------------------------------
                    ii = layer - 1; // set to last layer
                    // calculate the weighted sum
                    for (int i = 0; i < networkSize[ii]; i++)
                    {
                        for (int j = 1; j < inputsPerSample[ii]; j++)
                        {
                            sum = sum + weights[ii][i, j] * neuronOutputs[ii - 1][j - 1];
                        }
                        sum = sum + weights[ii][i, 0];
                        neuronOutputs[ii][i] = sum;
                        //weightedSum[ii][i] = sum;
                        sum = new Complex(0, 0);
                    } // end for

                    for (int jj = 0; jj < networkSize[ii]; jj++)
                    {
                        // calculate discrete output
                        // get angle
                        dNeuronOutputs[jj] = Math.Atan2(neuronOutputs[ii][jj].Imaginary, neuronOutputs[ii][jj].Real);
                        // if output is less than 0, add 2 pi to make it positive
                        if (dNeuronOutputs[jj] < 0)
                            dNeuronOutputs[jj] = 2 * Math.PI + dNeuronOutputs[jj];
                        // end if
                        // round
                        dNeuronOutputs[jj] = Math.Truncate(dNeuronOutputs[jj] / sectorSize);
                        //dOutputNeurons[jj] = Math.Floor(dOutputNeurons[jj]/bb);
                        // convert results to byte... did not work correctly, because it could be more than 255.  So let it be integer.
                        networkOutputs[aa, jj] = Convert.ToInt32(dNeuronOutputs[jj]);
                    } // end for
                    #endregion second to last layer
                } // end row for loop
                #endregion
                // -------------- END OUTPUT CALCULATION -----------------------------

                // -------------- BEGIN NET ERROR CALCULATION ------------------------
                #region GLOBAL ERROR CALCULATION
                // calculate NET error
                for (int aa = 0; aa < numberOfInputSamples; aa++)
                {
                    for (int i = 0; i < colInputs; i++)
                    {
                        networkErrors[aa] += Math.Pow((networkOutputs[aa, i] - desiredOutputs[aa, i]), 2);
                    }
                    networkErrors[aa] /= numberOfOutputs;
                    mse += networkErrors[aa];
                } // end for aa
                // calculate mse
                mse /= numberOfInputSamples;
                // calculate rmse
                rmse = Math.Sqrt(mse);
                window.SetText1("Iteration " + iteration + " done.          RMSE: " + rmse + Environment.NewLine);
                // Check if learning has converged
  //--->        window.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                // Action when cancel button is clicked
                if (cancelToken.IsCancellationRequested)
                    cancelToken.ThrowIfCancellationRequested();

                // Action when pause button is clicked
                await pauseToken.WaitWhilePausedAsync();

                if (rmse <= globalThreasholdValue)
                {
                    finishedLearning = true;
                    window.SetText1("\r\nLearning Converged!!!" + Environment.NewLine);
                }
                // end if
                #endregion

                // --------------- BEGIN LEARNING / MODIFICATION OF WEIGHTS ---------------------------
                // LEARNING / MODIFICATION OF WEIGHTS
                // if the algorithm has not finished learning then output of the
                // network needs to be calculated again to start correction of
                // errors
                #region OUTPUT CALCULATION
                if (!finishedLearning)
                {

                    // calculating the output of the network for each sample and
                    // correcting weights if output is > localThresholdValue
                    for (int aa = 0; aa < numberOfInputSamples; aa++) // for each row
                    {
                        #region calculate weighted sum of first layer and its activation
                        // ii holds current layer index. Process first layer
                        int ii = 0;

                        // calculate weighted sum for 1st hidden layer
                        for (int i = 0; i < networkSize[ii]; i++)
                        {
                            for (int j = 1; j < inputsPerSample[ii]; j++)
                            {
                                sum = sum + weights[ii][i, j] * Cinputs[aa, j - 1]; // <-- 7.2  of processing done here
                            }
                            sum = sum + weights[ii][i, 0];
                            //neuronOutputs[ii][i] = sum;
                            weightedSum[ii][i] = sum;
                            sum = new Complex(0, 0);
                        } // end for

                        // apply continuous activation
                        for (int t = 0; t < networkSize[ii]; t++)
                            neuronOutputs[ii][t] = weightedSum[ii][t] / Complex.Abs(weightedSum[ii][t]);
                        // end for
                        #endregion
                        #region calculate weighted sum of second to last hidden layer
                        // ----------------- Process second to last hidden layers, then output layer
                        // ii holds current layer
                        for (ii = 1; ii < layer - 1; ii++)
                        {
                            for (int i = 0; i < networkSize[ii]; i++)
                            {
                                for (int j = 1; j < inputsPerSample[ii]; j++)
                                {
                                    sum = sum + weights[ii][i, j] * neuronOutputs[ii - 1][j - 1];
                                }
                                sum = sum + weights[ii][i, 0];
                                //neuronOutputs[ii][i] = sum;
                                weightedSum[ii][i] = sum;
                                sum = new Complex(0, 0);
                            } // end for
                            // apply continuous activation
                            for (int t = 0; t < networkSize[ii]; t++)
                                neuronOutputs[ii][t] = weightedSum[ii][t] / Complex.Abs(weightedSum[ii][t]);
                            // end for
                        } // end for ii
                        #endregion
                        #region calculate output layer and network output calculation
                        // output layer
                        ii = layer - 1; // set to last layer
                        // calculate the weighted sum
                        for (int i = 0; i < networkSize[ii]; i++)
                        {
                            for (int j = 1; j < inputsPerSample[ii]; j++)
                            {
                                sum = sum + weights[ii][i, j] * neuronOutputs[ii - 1][j - 1]; // <-- 5.8 of processing is done here
                            }
                            sum = sum + weights[ii][i, 0];
                            neuronOutputs[ii][i] = sum;
                            weightedSum[ii][i] = sum;
                            sum = new Complex(0, 0);
                        } // end for

                        // apply the activation function for discrete outputs
                        for (int jj = 0; jj < networkSize[ii]; jj++)
                        {
                            // Action when cancel button is clicked
                            if (cancelToken.IsCancellationRequested)
                                cancelToken.ThrowIfCancellationRequested();

                            // Action when pause button is clicked
                            await pauseToken.WaitWhilePausedAsync();

                            // calculate discrete output
                            // get angle
                            dNeuronOutputs[jj] = Math.Atan2(weightedSum[ii][jj].Imaginary, weightedSum[ii][jj].Real);
                            // if output is less than 0, add 2 pi to make it positive
                            if (dNeuronOutputs[jj] < 0)
                                dNeuronOutputs[jj] = 2 * Math.PI + dNeuronOutputs[jj];
                            // end if
                            // round
                            dNeuronOutputs[jj] = Math.Truncate(dNeuronOutputs[jj] / sectorSize);
                            //dOutputNeurons[jj] = Math.Floor(dOutputNeurons[jj]/bb);
                            // convert results to byte... did not work correctly, because it could be more than 255.  So let it be integer.
                            networkOutputs[aa, jj] = Convert.ToInt32(dNeuronOutputs[jj]);
                        } // end for
                        #endregion second to last layer

                        #region R/MSE SPECIFIC CALCULATION OF ERROR
                        // calculate NET error
                        for (int i = 0; i < colInputs; i++)
                        {
                            networkErrors[aa] += Math.Pow((networkOutputs[aa, i]) - desiredOutputs[aa, i], 2);
                        }
                        networkErrors[aa] /= numberOfOutputs;
                        // calculate rmse
                        networkErrors[aa] = Math.Sqrt(networkErrors[aa]);
                        #endregion

                        #region Weights Modification
                        // check agains local threshold
                        // if localThresholdValue is greater, then the weights are corrected
                        if (localThresholdValue < networkErrors[aa])
                        {
                            // Action when cancel button is clicked
                            if (cancelToken.IsCancellationRequested)
                                cancelToken.ThrowIfCancellationRequested();

                            // Action when pause button is clicked
                            await pauseToken.WaitWhilePausedAsync();

                            #region ERROR CALCULATION
                            // CALCULATE THE ERROR OF THE NEURONS
                            // calculation of the errors of neurons starts at last layer
                            // and moves to first layer
                            ii = layer - 1;
                            // outputs will contain normalized weighted sums for all output neurons
                            for (int t = 0; t < networkSize[ii]; t++)
                                neuronOutputs[ii][t] = weightedSum[ii][t] / Complex.Abs(weightedSum[ii][t]);
                            // end for
                            // the global error for the jjj-th output neuron
                            // equals a root of unity corresponding to the
                            // desired output - normalized weighted sum for
                            // the corresponding output neuron
                            for (int jjj = 0; jjj < networkSize[ii]; jjj++)
                                neuronErrors[ii][jjj] = Sector[desiredOutputs[aa, jjj]] - neuronOutputs[ii][jjj];
                            // end for
                            // finally we obtain the output neurons' errors
                            // normalizing the global errors (dividing them
                            // by the (number of neurons in the preceding
                            // layer+1)
                            for (int t = 0; t < networkSize[ii]; t++)
                                neuronErrors[ii][t] = neuronErrors[ii][t] / (networkSize[ii - 1] + 1);
                            // end for

                            // ------------- HANDLING THE REST OF LAYERS - ERROR BACKPROPAGATION ------------
                            for (ii = layer - 2; -1 < ii; ii--)
                            {
                                // calculate the reciprocal weights for the layer ii and putting them in 
                                // a vector-row 
                                temp = new Complex[weights[ii + 1].GetLength(1) - 1, weights[ii + 1].GetLength(0)];
                                for (int i = 1; i < temp.GetLength(0) + 1; i++) //511
                                    for (int j = 0; j < temp.GetLength(1); j++) //169
                                        temp[i - 1, j] = 1 / weights[ii + 1][j, i]; // <-- 23.5 of processing done here
                                //end fors
                                // confirmed bug free above this point... 5/2/2014 0:48

                                sum = new Complex(0, 0);
                                // backpropagation of weights
                                if (0 < ii) // all hidden layers except the 1st
                                    for (int row = 0; row < temp.GetLength(0); row++)
                                    {
                                        for (int col = 0; col < temp.GetLength(1); col++)
                                        {
                                            sum = sum + temp[row, col] * neuronErrors[ii + 1][col];
                                        }
                                        neuronErrors[ii][row] = sum / (networkSize[ii - 1] + 1);
                                        sum = new Complex(0, 0);
                                        // end for
                                    } // end for
                                else
                                    for (int row = 0; row < temp.GetLength(0); row++)
                                    {
                                        for (int col = 0; col < temp.GetLength(1); col++)
                                        {
                                            sum = sum + temp[row, col] * neuronErrors[ii + 1][col];
                                        }
                                        neuronErrors[ii][row] = sum / (inputsPerSample[0]);
                                        sum = new Complex(0, 0);
                                        // end for
                                    } // end for
                                // end if
                            } // end ii for loop over the layers
                            #endregion

                            #region WEIGHTS CORRECTION
                            // -------------- CORRECTS THE WEIGHTS OF THE NETWORK ----------------------------------
                            // HANDLING THE 1ST HIDDEN LAYER
                            // learning rate is a reciprocal absolute value of the weighted sum
                            learningRate = new double[weightedSum[0].GetLength(0)];
                            for (int i = 0; i < learningRate.GetLength(0); i++)
                                learningRate[i] = (1 / Complex.Abs(weightedSum[0][i]));
                            // end for

                            // take learning rate into account
                            // for all weights except bias w0
                            for (int row = 0; row < networkSize[0]; row++)
                                for (int col = 1; col < inputsPerSample[0]; col++)
                                    weights[0][row, col] = weights[0][row, col] + (learningRate[row] * neuronErrors[0][row]) * Complex.Conjugate(Cinputs[aa, col - 1]);
                            // end for
                            for (int row = 0; row < networkSize[0]; row++)
                                weights[0][row, 0] = weights[0][row, 0] + (learningRate[row] * neuronErrors[0][row]);
                            // end for


                            // correct the following layers
                            for (ii = 1; ii < layer; ii++)
                            {
                                sum = new Complex(0, 0);
                                // calculate the new output of preceding layer
                                // if preceding layer is the 1st one
                                if (ii == 1)
                                {
                                    // calculate weighted sum
                                    for (int i = 0; i < networkSize[0]; i++)
                                    {
                                        for (int j = 1; j < inputsPerSample[0]; j++)
                                        {
                                            sum = sum + weights[0][i, j] * Cinputs[aa, j - 1];
                                        }
                                        sum = sum + weights[0][i, 0];
                                        weightedSum[0][i] = sum;
                                        sum = new Complex(0, 0);
                                    } // end for
                                } // end if
                                else // if a preceding layer is not the 1st one
                                {
                                    // calculate weighted sum & activation
                                    for (int i = 0; i < networkSize[ii - 1]; i++)
                                    {
                                        for (int j = 1; j < inputsPerSample[ii - 1]; j++)
                                        {
                                            sum = sum + weights[ii - 1][i, j] * neuronOutputs[ii - 2][j - 1];
                                        }
                                        sum = sum + weights[ii - 1][i, 0];
                                        weightedSum[ii - 1][i] = sum;
                                        sum = new Complex(0, 0);
                                    } // end for
                                } // end if

                                // CONTINUOUS OUTPUT CALCULATION
                                for (int t = 0; t < networkSize[ii - 1]; t++)
                                    neuronOutputs[ii - 1][t] = weightedSum[ii - 1][t] / Complex.Abs(weightedSum[ii - 1][t]);
                                // end for

                                // learning rate is the reciprocal absolute value of the weighted sum
                                learningRate = new double[weightedSum[ii].GetLength(0)];
                                for (int i = 0; i < learningRate.GetLength(0); i++)
                                    learningRate[i] = (1 / Complex.Abs(weightedSum[ii][i]));
                                // end for

                                // learning rate not used for the output layer neurons
                                if (ii < layer) // <-- ~10  of processing done here (partly due to complex calculations)
                                {
                                    // do last things
                                    a1 = (Complex[,])weights[ii].Clone();
                                    b1 = (Complex[])neuronErrors[ii].Clone();
                                    for (int i = 0; i < b1.GetLength(0); i++)
                                        b1[i] = b1[i] * learningRate[i];
                                    // end for
                                    c1 = (Complex[])neuronOutputs[ii - 1].Clone();
                                    for (int i = 0; i < c1.GetLength(0); i++)
                                        c1[i] = Complex.Conjugate(c1[i]);
                                    // end for
                                    e1 = (Complex[,])a1.Clone();
                                    for (int i = 0; i < networkSize[ii]; i++)
                                    {
                                        d1 = b1[i];
                                        for (int j = 1; j < a1.GetLength(1); j++)
                                            e1[i, j] = d1 * c1[j - 1];
                                    }
                                    f1 = new Complex[a1.GetLength(0), a1.GetLength(1) - 1];
                                    for (int i = 0; i < a1.GetLength(0); i++)
                                        for (int j = 1; j < a1.GetLength(1); j++)
                                            f1[i, j - 1] = a1[i, j] + e1[i, j];
                                    // end for

                                    for (int i = 0; i < weights[ii].GetLength(0); i++)
                                        for (int j = 1; j < weights[ii].GetLength(1); j++)
                                            weights[ii][i, j] = f1[i, j - 1];
                                    // end for
                                    for (int i = 0; i < weights[ii].GetLength(0); i++)
                                        weights[ii][i, 0] = weights[ii][i, 0] + (learningRate[i] * neuronErrors[ii][i]);
                                    // end for
                                }
                                else
                                {
                                    // do last things
                                    a1 = (Complex[,])weights[ii].Clone();
                                    b1 = (Complex[])neuronErrors[ii].Clone();
                                    c1 = (Complex[])neuronOutputs[ii - 1].Clone();
                                    for (int i = 0; i < c1.GetLength(0); i++)
                                        c1[i] = Complex.Conjugate(c1[i]);
                                    // end for
                                    e1 = (Complex[,])a1.Clone();
                                    for (int i = 0; i < networkSize[ii]; i++)
                                    {
                                        d1 = b1[i];
                                        for (int j = 1; j < a1.GetLength(1); j++)
                                            e1[i, j] = d1 * c1[j - 1];
                                    }
                                    f1 = new Complex[a1.GetLength(0), a1.GetLength(1) - 1];
                                    for (int i = 0; i < a1.GetLength(0); i++)
                                        for (int j = 1; j < a1.GetLength(1); j++)
                                            f1[i, j - 1] = a1[i, j] + e1[i, j];
                                    // end for

                                    for (int i = 0; i < weights[ii].GetLength(0); i++)
                                        for (int j = 1; j < weights[ii].GetLength(1); j++)
                                            weights[ii][i, j] = f1[i, j - 1];
                                    // end for
                                    for (int i = 0; i < weights[ii].GetLength(0); i++)
                                        weights[ii][i, 0] = weights[ii][i, 0] + neuronErrors[ii][i];
                                    // end for
                                } // end if
                            } // end ii for
                            #endregion
                        #endregion
                        } // end localThresholdValue if check
                    } // end aa for loop
                } // end ~finishedLearning if statement
                #endregion
            }// end ~finishedLearning while loop
            #endregion

            return weights;
        }

        public static byte[,] loadLearningSamples(string fileName, int numberOfSamples, int inputsPerSample)
        {
            // create multidimensional array to store samples
            byte[,] samples = new byte[numberOfSamples, inputsPerSample];
            // initialize the network to zeros

            for (int i = 0; i < numberOfSamples; i++)
            {
                for (int j = 0; j < inputsPerSample; j++)
                    samples[i, j] = 0;
            } // end for

            // Load the weights
            TextReader b = File.OpenText(fileName);
            string temp;
            string[] bits;
            int jj = 0;
            int ii = 0;
            bool isNum = false;
            int check = 0;
            for (int i = 0; i < numberOfSamples; i++)
            {
                ii = 0;
                jj = 0;
                temp = b.ReadLine();
                bits = temp.Split('\t', ' ');
                while (jj < inputsPerSample)
                {
                    isNum = int.TryParse(bits[ii].ToString(), out check);
                    if (isNum)
                    {
                        samples[i, jj] = (byte)check;
                        jj++;
                        ii++;
                    }
                    else
                        ii++;
                    // end if
                } // end while
            } // end for i

            b.Dispose();
            return samples;
        }
    }
}

