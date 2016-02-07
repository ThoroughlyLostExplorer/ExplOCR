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
    class TrainingConfig
    {
        // Train the neural networks. 
        //Note: This depends on the way your knowlwdge directory is set up and may need to be changed whenever retraining!
        public static void TrainNN(out OcrReader ocrReader)
        {
            int dimensionX = Properties.Settings.Default.DimensionX;
            int dimensionY = Properties.Settings.Default.DimensionY;

            NeuralNet nnDescriptions, nnTables, nnNumbers, nnHeadlines, nnDelimiters;

            List<string> knowledge = new List<string>();
            List<char> netKeys = new List<char>();
            string files = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(PathHelpers.BuildKnowledgeFilename(DescriptionsNetwork, files[i].ToString() + "_lower")))
                {
                    knowledge.Add(PathHelpers.BuildKnowledgeFilename(DescriptionsNetwork, files[i].ToString() + "_lower"));
                    netKeys.Add(files[i]);
                }
                if (File.Exists(PathHelpers.BuildKnowledgeFilename(DescriptionsNetwork, files[i].ToString() + "_upper")))
                {
                    knowledge.Add(PathHelpers.BuildKnowledgeFilename(DescriptionsNetwork, files[i].ToString() + "_upper"));
                    netKeys.Add(char.ToUpper(files[i]));
                }
            }
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(DescriptionsNetwork, "delimiter"));
            netKeys.Add('.');
            netKeys.Add('#');
            netKeys.Add('-');

            nnDescriptions = new NeuralNet(dimensionX, dimensionY, netKeys, knowledge);
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

            nnTables = new NeuralNet(dimensionX, dimensionY, netKeys, knowledge);
            nnTables.SaveFile = PathHelpers.BuildNetworkFilename(TablesNetwork);
            nnTables.Train(Properties.Settings.Default.SamplesTables);

            knowledge.Clear();
            netKeys.Clear();

            knowledge.Add(PathHelpers.BuildKnowledgeFilename(DelimitersNetwork, "comma"));
            netKeys.Add('#');
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(DelimitersNetwork, "dot"));
            netKeys.Add('.');
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(DelimitersNetwork, "minus"));
            netKeys.Add('-');
            nnDelimiters = new NeuralNet(dimensionX, dimensionY, netKeys, knowledge);
            nnDelimiters.SaveFile = PathHelpers.BuildNetworkFilename(DelimitersNetwork);
            nnDelimiters.Train(Properties.Settings.Default.SamplesNumbers);

            knowledge.Clear();
            netKeys.Clear();

            files = "0123456789";
            for (int i = 0; i < files.Length; i++)
            {
                knowledge.Add(PathHelpers.BuildKnowledgeFilename(NumbersNetwork, files[i].ToString()));
                netKeys.Add(files[i]);
            }
            nnNumbers = new NeuralNet(dimensionX, dimensionY, netKeys, knowledge);
            nnNumbers.SaveFile = PathHelpers.BuildNetworkFilename(NumbersNetwork);
            nnNumbers.Train(Properties.Settings.Default.SamplesNumbers);

            knowledge.Clear();
            netKeys.Clear();

            files = "0123456789";
            for (int i = 0; i < files.Length; i++)
            {
                knowledge.Add(PathHelpers.BuildKnowledgeFilename(HeadlinesNetwork, files[i].ToString()));
                netKeys.Add(files[i]);
            }
            files = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
            for (int i = 0; i < files.Length; i++)
            {
                knowledge.Add(PathHelpers.BuildKnowledgeFilename(HeadlinesNetwork, files[i].ToString() + "_upper"));
                netKeys.Add(files[i]);
            }
            files = "abcdefghijklmnopqrstuvwyz";
            for (int i = 0; i < files.Length; i++)
            {
                knowledge.Add(PathHelpers.BuildKnowledgeFilename(HeadlinesNetwork, files[i].ToString() + "_lower"));
                netKeys.Add(files[i]);
            }
            knowledge.Add(PathHelpers.BuildKnowledgeFilename(HeadlinesNetwork, "minus"));
            netKeys.Add('-');

            nnHeadlines = new NeuralNet(15, 22, netKeys, knowledge);
            nnHeadlines.SaveFile = PathHelpers.BuildNetworkFilename(HeadlinesNetwork);
            nnHeadlines.Factor = 2;
            nnHeadlines.Train(Properties.Settings.Default.SamplesHeadlines);

            ocrReader = new OcrReader(nnDescriptions, nnTables, nnNumbers,nnHeadlines, nnDelimiters);
        }

        const string NumbersNetwork = "numbers";
        const string DelimitersNetwork = "delimiters";
        const string DescriptionsNetwork = "descriptions";
        const string TablesNetwork = "tables";
        const string HeadlinesNetwork = "headlines";
    }
}
