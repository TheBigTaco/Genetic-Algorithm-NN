using System.Collections.Generic;
using UnityEngine;

namespace NeuralNetwork
{
    public class Manager : MonoBehaviour
    {
        public GameObject AgentPrefab;
        public GameObject Target;

        public int PopulationSize;
        public int Epoch = 0;
        public int TargetHits = 0;
        public bool Learning = false;

        public List<NeuralNetwork> Networks = new List<NeuralNetwork>();
        public List<Agent> Agents = new List<Agent>();

        void Start()
        {
            CreateNetworks();
        }

        void Update()
        {
            if (!Learning)
            {
                if (Epoch != 0)
                {
                    Networks.Sort();
                    for (int i = 0; i < PopulationSize; i++)
                    {
                        //print(Networks[i].Fitness);
                    }

                    for (int i = 0; i < PopulationSize / 2; i++)
                    {
                        Networks[i] = new NeuralNetwork(Networks[i + PopulationSize / 2]);
                        Networks[i].MutateWeights();
                    }

                    for (int i = PopulationSize / 2; i < PopulationSize; i++)
                    {
                        Networks[i] = new NeuralNetwork(Networks[i]);
                    }

                    for (int i = 0; i < PopulationSize; i++)
                    {
                        Networks[i].Fitness = 0f;
                    }
                }
                foreach(Agent agent in Agents)
                {
                    if(agent.TargetHit)
                    {
                        TargetHits++;
                    }
                }
                print("##########################"+TargetHits);
                TargetHits = 0;
                Epoch++;

                Agents = new List<Agent>();
                CreateAgents();

                Learning = true;

                Invoke("Lifetime", 150f);
            }
        }

        public void CreateNetworks()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                NeuralNetwork newNet = new NeuralNetwork(new int[4] { 2, 3, 3, 2 });
                Networks.Add(newNet);
            }
        }

        public void CreateAgents()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                Agent agent = Instantiate(AgentPrefab).GetComponent<Agent>();

                agent.GivePurpose(Networks[i], Target.transform);

                agent.Brain.Fitness = 0;

                Agents.Add(agent);
            }
        }

        public void Lifetime()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                GameObject.Destroy(Agents[i].gameObject);
            }

            Learning = false;
        }
    }
}