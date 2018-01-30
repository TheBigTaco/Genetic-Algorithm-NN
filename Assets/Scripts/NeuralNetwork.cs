using System;
using System.Collections.Generic;

namespace NeuralNetwork
{
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        public int[] Layers; // e.g. input, hidden, output
        public float[][] Nodes; // e.g. 2 inputs, n hidden, 2 output
        public float[][][] Weights; // the bias towards n action
        public float Fitness { get; set; } //Increased when neural net does something good, decreased when bad
        // First Constructor
        public NeuralNetwork(int[] layerInfo)
        {
            Layers = new int[layerInfo.Length];

            for (int i = 0; i < layerInfo.Length; i++)
            {
                Layers[i] = layerInfo[i];
            }

            CreateNodeMatrix();
            CreateWeightMatrix();
        }
        // Recursion Constructor for learning
        public NeuralNetwork(NeuralNetwork otherNetwork)
        {
            Layers = new int[otherNetwork.Layers.Length];

            for (int i = 0; i < otherNetwork.Layers.Length; i++)
            {
                Layers[i] = otherNetwork.Layers[i];
            }

            CreateNodeMatrix();
            CreateWeightMatrix();
            WeightCopy(otherNetwork.Weights);
        }
        // Get outputs from inputs
        public float[] FeedForward(float[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                Nodes[0][i] = inputs[i];
            }
            
            for (int i = 1; i < Layers.Length; i++)
            {
                for (int j = 0; j < Nodes[i].Length; j++)
                {
                    float value = 0.25f;
                    
                    for (int k = 0; k < Nodes[i - 1].Length; k++)
                    {
                        value += Weights[i - 1][j][k] * Nodes[i - 1][k];
                    }

                    Nodes[i][j] = (float)Math.Tanh(value);
                }
            }

            return Nodes[Nodes.Length - 1];
        }
        // Change bias
        public void MutateWeights()
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                for (int j = 0; j < Weights[i].Length; j++)
                {
                    for (int k = 0; k < Weights[i][j].Length; k++)
                    {
                        float Weight = Weights[i][j][k];
                        
                        float rand = UnityEngine.Random.Range(0f, 100f);

                        if (rand <= 2f)
                        {
                            Weight *= -1f;
                        }
                        else if (rand <= 4f)
                        {
                            Weight = UnityEngine.Random.Range(-5f, 5f);
                        }
                        else if (rand <= 6f)
                        {
                            float maximize = UnityEngine.Random.Range(1f, 2f);

                            Weight *= maximize;
                        }
                        else if (rand <= 8f)
                        {
                            float minimize = UnityEngine.Random.Range(0f, 1f);
                            Weight *= minimize;
                        }

                        Weights[i][j][k] = Weight;
                    }
                }
            }
        }
        // Make Nodes
        private void CreateNodeMatrix()
        {
            List<float[]> nodeList = new List<float[]>();

            for (int i = 0; i < Layers.Length; i++)
            {
                nodeList.Add(new float[Layers[i]]);
            }

            Nodes = nodeList.ToArray();
        }
        // Make Weights
        private void CreateWeightMatrix()
        {
            List<float[][]> weightList = new List<float[][]>();

            for (int i = 1; i < Layers.Length; i++)
            {
                List<float[]> currentLayerWeights = new List<float[]>();

                int nodesInPreviousLayer = Layers[i - 1];

                for (int j = 0; j < Nodes[i].Length; j++)
                {
                    float[] nodeWeights = new float[nodesInPreviousLayer];

                    for (int k = 0; k < nodesInPreviousLayer; k++)
                    {
                        nodeWeights[k] = UnityEngine.Random.Range(-5f, 5f);
                    }

                    currentLayerWeights.Add(nodeWeights);
                }

                weightList.Add(currentLayerWeights.ToArray());
            }

            Weights = weightList.ToArray();
        }
        // Used in recursion to keep bias the same
        public void WeightCopy(float[][][] weightCopy)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                for (int j = 0; j < Weights[i].Length; j++)
                {
                    for (int k = 0; k < Weights[i][j].Length; k++)
                    {
                        Weights[i][j][k] = weightCopy[i][j][k];
                    }
                }
            }
        }

        // Change Fitness
        public void AddFitness(float newFitness)
        {
            Fitness += newFitness;
        }
        // Compare two NeuralNetwork's Fitness to eachother
        public int CompareTo(NeuralNetwork other)
        {
            if (other == null)
            {
                return 1;
            }
            if (Fitness > other.Fitness)
            {
                return 1;
            }
            else if (Fitness < other.Fitness)
            {
                return -1;
            }
            else
            {
                return 0;
            }

        }
    }
}