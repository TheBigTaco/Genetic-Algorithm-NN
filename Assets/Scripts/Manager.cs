using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NeuralNetwork
{
    public class Manager : MonoBehaviour
    {
        public GameObject AgentPrefab; //Agent Object to clone to population size
        public GameObject Target; //Where Agents want to go

        public int PopulationSize; //Can change this in unity on the Manager Object
        public int Epoch = 0; //How many resets/generations have gone by
        public int TargetHits = 0; //How many agents have hit the target this epoch
        public bool Learning = false;

        public List<NeuralNetwork> Networks = new List<NeuralNetwork>(); //List of brains
        public List<Agent> Agents = new List<Agent>(); //List of bodies
        public List<Agent> OldAgents = new List<Agent>();

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
                    OldAgents = Agents;
                    Networks.Sort(); //Sort by fitness, low to high
                    for (int i = 0; i < PopulationSize; i++)
                    {
                        //print(Networks[i].Fitness);
                    }
                    //Split population into best half and worst half
                    for (int i = 0; i < PopulationSize / 2; i++)
                    {
                        //Set worst half to equal top half, mutate them
                        Networks[i] = new NeuralNetwork(Networks[i + PopulationSize / 2]);
                        Networks[i].MutateWeights();
                    }

                    for (int i = PopulationSize / 2; i < PopulationSize; i++)
                    {
                        //Keep top half brains the same
                        Networks[i] = new NeuralNetwork(Networks[i]);
                    }
                    
                }
                //Count how many agents hit the target per epoch for Log purposes
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
                //Call the liftime method every nf seconds
                Invoke("Lifetime", 130f);
            }
        }

        public void CreateNetworks()
        {
            //Create initial brains
            for (int i = 0; i < PopulationSize; i++)
            {
                NeuralNetwork newNet = new NeuralNetwork(new int[4] { 2, 3, 3, 2 });
                Networks.Add(newNet);
            }
        }

        public void CreateAgents()
        {
            print(OldAgents.Count);
            //Make rigid bodies for brains
            for (int i = 0; i < PopulationSize; i++)
            {
                Agent agent = Instantiate(AgentPrefab).GetComponent<Agent>();

                agent.GivePurpose(Networks[i], Target.transform);

                
                //Change color of Agent based on fitness
                if(Epoch != 1)
                {
                    OldAgents.OrderByDescending(a => a.Brain.Fitness);
                    var rend = agent.GetComponent<MeshRenderer>();
                    if (OldAgents[i].Brain.Fitness <= -20)
                    {
                        rend.material = Resources.Load("Materials/Sad-face", typeof(Material)) as Material;
                        rend.material.color = Color.red;
                    }
                    else if (OldAgents[i].Brain.Fitness <= 10)
                    {
                        rend.material = Resources.Load("Materials/nuterall", typeof(Material)) as Material;
                        rend.material.color = Color.yellow;
                    }
                    else if (OldAgents[i].Brain.Fitness <= 50)
                    {
                        rend.material = Resources.Load("Materials/Smiley_Face", typeof(Material)) as Material;
                        rend.material.color = Color.green;
                    }
                    else
                    {
                        rend.material = Resources.Load("Materials/mustache", typeof(Material)) as Material;
                        rend.material.color = Color.magenta;
                    }
                }
                agent.Brain.Fitness = 0;
                Agents.Add(agent);
            }
        }

        public void Lifetime()
        {
            //Destroy bodies, set learning to false in order for new bodies to be created
            for (int i = 0; i < PopulationSize; i++)
            {
                GameObject.Destroy(Agents[i].gameObject);
            }

            Learning = false;
        }
    }
}