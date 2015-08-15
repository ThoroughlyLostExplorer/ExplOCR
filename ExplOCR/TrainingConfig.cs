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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplOCR
{
    class TrainingConfig
    {
        // Train the neural networks. 
        //Note: This depends on the way your knowlwdge directory is set up and may need to be changed whenever retraining!
        public static void TrainNN(out OcrReader ocrReader)
        {
            NeuralNet nnDescriptions, nnTables, nnNumbers;

            List<string> knowledge = new List<string>();
            List<char> netKeys = new List<char>();
            //string files = "0123456789";
            string files = "abcdefghiklmnopqrstuvwy";
            for (int i = 0; i < files.Length; i++)
            {
                knowledge.Add(PathHelpers.BuildKnowledgeFilename(DescriptionsNetwork, files[i].ToString() + "_lower"));
                netKeys.Add(files[i]);
            }
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(DescriptionsNetwork, "delimiter"));
            netKeys.Add('.');
            netKeys.Add('#');

            nnDescriptions = new NeuralNet(DimensionX, DimensionY, netKeys, knowledge);
            nnDescriptions.SaveFile = PathHelpers.BuildNetworkFilename(DescriptionsNetwork);
            nnDescriptions.Train(Properties.Settings.Default.SamplesDescriptions);

            knowledge.Clear();
            netKeys.Clear();

            files = "ABCDEFGHIJKLMNOPRSTUVWXY";
            for (int i = 0; i < files.Length; i++)
            {
                knowledge.Add(PathHelpers.BuildKnowledgeFilename(TablesNetwork, files[i].ToString() + "_upper"));
                netKeys.Add(files[i]);
            }
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(TablesNetwork, "delimiter"));
            netKeys.Add(':');
            netKeys.Add('(');
            netKeys.Add(')');
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(TablesNetwork, "minus"));
            netKeys.Add('-');

            nnTables = new NeuralNet(DimensionX, DimensionY, netKeys, knowledge);
            nnTables.SaveFile = PathHelpers.BuildNetworkFilename(TablesNetwork);
            nnTables.Train(Properties.Settings.Default.SamplesTables);

            knowledge.Clear();
            netKeys.Clear();

            files = "0123456789";
            for (int i = 0; i < files.Length; i++)
            {
                knowledge.Add(PathHelpers.BuildKnowledgeFilename(NumbersNetwork, files[i].ToString()));
                netKeys.Add(files[i]);
            }
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(NumbersNetwork, "comma"));
            netKeys.Add('#');
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(NumbersNetwork, "dot"));
            netKeys.Add('.');
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(NumbersNetwork, "minus"));
            netKeys.Add('-');

            nnNumbers = new NeuralNet(DimensionX, DimensionY, netKeys, knowledge);
            nnNumbers.SaveFile = PathHelpers.BuildNetworkFilename(NumbersNetwork);
            nnNumbers.Train(Properties.Settings.Default.SamplesNumbers);

            ocrReader = new OcrReader(nnDescriptions, nnTables, nnNumbers);
        }

        public static int DimensionX
        {
            get { return Properties.Settings.Default.DimensionX; }
        }
        public static int DimensionY
        {
            get { return Properties.Settings.Default.DimensionY; }
        }

        const string NumbersNetwork = "numbers";
        const string DescriptionsNetwork = "descriptions";
        const string TablesNetwork = "tables";
    }
}
