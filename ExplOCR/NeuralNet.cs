// Copyright 2015 by the person represented as ThoroughlyLostExplorer on GitHub
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplOCR
{
    class NeuralNet : IDisposable
    {
        public NeuralNet(int dX, int dY, IEnumerable<char> chars, IEnumerable<string> files)
        {
            netKeys = new List<char>(chars);
            knowledgeFiles = new List<string>(files);
            dimensionX = dX;
            dimensionY = dY;
        }

        public string SaveFile
        {
            get { return saveFile; }
            set { saveFile = value; }
        }

        public void Train(int samples)
        {
            if (samples > 0 && TryLoadTrainingSet(samples))
            {
                return;
            }

            List<string> knowledge = new List<string>();
            foreach (string file in knowledgeFiles)
            {
                knowledge.AddRange(File.ReadAllLines(file));
            }

            string[] lines = knowledge.ToArray();
            samples = lines.Length; ;
            int pixels = DimensionY * DimensionX;
            int classes = netKeys.Count;

            Emgu.CV.Matrix<Single> training = new Emgu.CV.Matrix<Single>(samples, pixels);
            Emgu.CV.Matrix<Single> class_training = new Emgu.CV.Matrix<Single>(samples, classes);

            Emgu.CV.Matrix<int> layers = new Emgu.CV.Matrix<int>(3, 1);
            layers[0, 0] = pixels;
            layers[1, 0] = (int)(1.5 * netKeys.Count);
            layers[2, 0] = classes;

            for (int i = 0; i < samples; i++)
            {
                LetterInfo info = LetterInfo.ReadLetterInfoLine(lines[i]);
                byte[] bytes = Convert.FromBase64String(info.Base64);
                for (int j = 0; j < pixels; j++)
                {
                    training[i, j] = bytes[j];
                }
                int d = netKeys.IndexOf(info.Char);
                class_training[i, d] = 1;
            }

            nnet = new Emgu.CV.ML.ANN_MLP(layers, Emgu.CV.ML.MlEnum.ANN_MLP_ACTIVATION_FUNCTION.SIGMOID_SYM, 0.6, 1);
            Emgu.CV.ML.Structure.MCvANN_MLP_TrainParams p = new Emgu.CV.ML.Structure.MCvANN_MLP_TrainParams();
            p.term_crit.type = Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_EPS | Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_ITER;
            p.term_crit.max_iter = 1000;
            p.term_crit.epsilon = 0.000001;
            p.train_method = Emgu.CV.ML.MlEnum.ANN_MLP_TRAIN_METHOD.BACKPROP;
            p.bp_dw_scale = 0.1;
            p.bp_moment_scale = 0.1;

            bool success = false;
            try
            {
                if (File.Exists(saveFile))
                {
                    nnet.Load(saveFile);
                    success = true;
                }
            }
            catch
            {
            }

            if (!success)
            {
                int iteration = nnet.Train(training, class_training, null, p, Emgu.CV.ML.MlEnum.ANN_MLP_TRAINING_FLAG.DEFAULT);
                if (saveFile != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(saveFile));
                    nnet.Save(saveFile);
                }
            }
        }

        private bool TryLoadTrainingSet(int samples)
        {
            int pixels = DimensionY * DimensionX;
            int classes = netKeys.Count;

            Emgu.CV.Matrix<Single> training = new Emgu.CV.Matrix<Single>(samples, pixels);
            Emgu.CV.Matrix<Single> class_training = new Emgu.CV.Matrix<Single>(samples, classes);

            Emgu.CV.Matrix<int> layers = new Emgu.CV.Matrix<int>(3, 1);
            layers[0, 0] = pixels;
            layers[1, 0] = (int)(1.5 * netKeys.Count);
            layers[2, 0] = classes;

            nnet = new Emgu.CV.ML.ANN_MLP(layers, Emgu.CV.ML.MlEnum.ANN_MLP_ACTIVATION_FUNCTION.SIGMOID_SYM, 0.6, 1);
            Emgu.CV.ML.Structure.MCvANN_MLP_TrainParams p = new Emgu.CV.ML.Structure.MCvANN_MLP_TrainParams();
            p.term_crit.type = Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_EPS | Emgu.CV.CvEnum.TERMCRIT.CV_TERMCRIT_ITER;
            p.term_crit.max_iter = 1000;
            p.term_crit.epsilon = 0.000001;
            p.train_method = Emgu.CV.ML.MlEnum.ANN_MLP_TRAIN_METHOD.BACKPROP;
            p.bp_dw_scale = 0.1;
            p.bp_moment_scale = 0.1;

            try
            {
                if (File.Exists(saveFile))
                {
                    nnet.Load(saveFile);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        public int DimensionX
        {
            get { return dimensionX; }
        }

        public int DimensionY
        {
            get { return dimensionY; }
        }

        public char[] NetKeys
        {
            get { return netKeys.ToArray(); }
        }

        internal float[] PredictDetailed(byte[] bytes)
        {

            int pixels = DimensionX * DimensionY;
            int samples = 1;
            int classes = netKeys.Count;
            Emgu.CV.Matrix<Single> test = new Emgu.CV.Matrix<Single>(samples, pixels);
            Emgu.CV.Matrix<Single> class_test = new Emgu.CV.Matrix<Single>(samples, classes);
            Emgu.CV.Matrix<Single> result = new Emgu.CV.Matrix<float>(1, classes);

            for (int j = 0; j < pixels; j++)
            {
                test[0, j] = bytes[j];
            }

            float[] floats = new float[classes];
            Emgu.CV.Matrix<Single> sample = test.GetRow(0);
            nnet.Predict(sample, result);

            for (int j = 0; j < result.Cols; j++)
            {
                floats[j] = result[0, j];
            }
            return floats;
        }

        internal char Predict(byte[] bytes, bool margin)
        {
            int pixels = DimensionX * DimensionY;
            int samples = 1;
            int classes = netKeys.Count;
            Emgu.CV.Matrix<Single> test = new Emgu.CV.Matrix<Single>(samples, pixels);
            Emgu.CV.Matrix<Single> class_test = new Emgu.CV.Matrix<Single>(samples, classes);
            Emgu.CV.Matrix<Single> result = new Emgu.CV.Matrix<float>(1, classes);

            for (int j = 0; j < pixels; j++)
            {
                test[0, j] = bytes[j];
            }

            float max, max2;
            int max_idx;
            Emgu.CV.Matrix<Single> sample = test.GetRow(0);
            nnet.Predict(sample, result);

            max_idx = 0;
            max = result[0, 0];
            max2 = 0;
            for (int j = 0; j < classes; j++)
            {
                if (result[0, j] > max)
                {
                    max_idx = j;
                    max = result[0, j];
                }
            }
            for (int j = 0; j < classes; j++)
            {
                if (result[0, j] > max2 && j != max_idx)
                {
                    max2 = result[0, j];
                }
            }
            if (margin && Math.Abs(max - max2) < 0.1 && max > 0.5)
                return '*';
            if (max > 0.5) return netKeys[max_idx];
            //if (max > 5 * max2) return netKeys[max_idx];
            return '*';
        }

        public void Dispose()
        {
            if (nnet != null)
            {
                nnet.Dispose();
            }
        }

        Emgu.CV.ML.ANN_MLP nnet;
        List<char> netKeys;
        List<string> knowledgeFiles;
        readonly int dimensionX;
        readonly int dimensionY;
        string saveFile;
    }
}
