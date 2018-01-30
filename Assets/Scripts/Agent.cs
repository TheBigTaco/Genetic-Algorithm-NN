using System;
using UnityEngine;

namespace NeuralNetwork
{
    public class Agent : MonoBehaviour
    {
        public NeuralNetwork Brain;

        public Transform Target;

        public Rigidbody rigidBody; //The Ball

        public bool Alive = false; //Gaining fitness or not
        public float currentDistance; //How close it is to target this frame
        public float lastDistance = 0; //How close it was to target last frame
        public float currentFitness; //It's brains current fitness
        public bool TargetHit = false; //If it hit the target
        public float spawnTime;

        RaycastHit View;

        void Start()
        {
            spawnTime = Time.time; //The time that the ball was spawned in the world
            Physics.IgnoreLayerCollision(8, 8); //Balls ignore collision with eachother, else the physics launches them
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == Target.name) //Check if it collided with the target
            {
                TargetHit = true;
                rigidBody.velocity = Vector3.zero; //Stop it's velocity
                var timeToTarget = Time.time - spawnTime; //How much time it took to hit the target
                Brain.AddFitness(3); //Add 3 fitness for hitting the target
                Brain.AddFitness(130 - timeToTarget); //Add extra time to fitness
                print(Brain.Fitness);
            }
            else
            {
                Brain.AddFitness(-1); //punish colliding with walls
            }
        }

        void FixedUpdate()
        {
            //Push block away from the walls, keep it floating at y axis 1
            if (transform.position.y != 1)
            {
                transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            }

            if (transform.position.x < -4.5f)
            {
                rigidBody.AddForce(new Vector3(0.5f, 0f, 0f));
            }
            else if (transform.position.x > 4.5f)
            {
                rigidBody.AddForce(new Vector3(-0.5f, 0f, 0f));
            }

            if (transform.position.z < -4.5f)
            {
                rigidBody.AddForce(new Vector3(0f, 0f, 0.5f));
            }
            else if (transform.position.z > 34.5f)
            {
                rigidBody.AddForce(new Vector3(0f, 0f, -0.5f));
            }
            //Add Fitness
            if (Alive)
            {
                if (!TargetHit) //Check if it hit target, if it did, stop applying movement
                {
                    //Current position of the Agent
                    float[] inputs = new float[2];
                    inputs[0] = transform.position.x;
                    inputs[1] = transform.position.z;
                    //Feed into Neural Network
                    float[] output = Brain.FeedForward(inputs);
                    //New Position to move too
                    float outX = transform.position.x + output[0];
                    float outZ = transform.position.z + output[1];

                    Vector3 dirChange = new Vector3(outX, 1, outZ);
                    //Movement, and smoothness of movement
                    transform.position = Vector3.Lerp(transform.position, dirChange, 0.02f); 
                }
                //Determine distance moved and add to fitness
                Vector3 origin = transform.position;
                Vector3 dest = Target.position;
                Vector3 direction = dest - origin;
                currentDistance = direction.magnitude;
                Brain.AddFitness(lastDistance - currentDistance);
                currentFitness = Brain.Fitness;
            }
            lastDistance = currentDistance;
            
        }
        //Start Agent learning
        public void GivePurpose(NeuralNetwork brain, Transform target)
        {
            Brain = brain;
            Target = target;
            Alive = true;
        }
    }
}