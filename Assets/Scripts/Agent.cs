using System;
using UnityEngine;

namespace NeuralNetwork
{
    public class Agent : MonoBehaviour
    {
        public NeuralNetwork Brain;

        public Transform Target;

        public Rigidbody rigidBody;

        public bool Alive = false;
        public float currentDistance;
        public float lastDistance = 0;
        public float currentFitness;
        public bool TargetHit = false;
        public float spawnTime;

        RaycastHit View;

        void Start()
        {
            spawnTime = Time.time;
            Physics.IgnoreLayerCollision(8, 8);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == Target.name)
            {
                TargetHit = true;
                rigidBody.velocity = Vector3.zero;
                var timeToTarget = Time.time - spawnTime;
                Brain.AddFitness(3);
                Brain.AddFitness(150 - timeToTarget);
                print(Brain.Fitness);
            }
            else
            {
                Brain.AddFitness(-1);
            }
        }

        void FixedUpdate()
        {
            
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

            if (Alive)
            {
                if (!TargetHit)
                {
                    float[] inputs = new float[2];
                    inputs[0] = transform.position.x;
                    inputs[1] = transform.position.z;

                    float[] output = Brain.FeedForward(inputs);

                    float outX = transform.position.x + output[0];
                    float outZ = transform.position.z + output[1];

                    Vector3 dirChange = new Vector3(outX, 1, outZ);
                    transform.position = Vector3.Lerp(transform.position, dirChange, 0.02f);
                }
                Vector3 origin = transform.position;
                Vector3 dest = Target.position;
                Vector3 direction = dest - origin;
                currentDistance = direction.magnitude;
                Brain.AddFitness(lastDistance - currentDistance);
                currentFitness = Brain.Fitness;
                //if (Physics.Raycast(transform.position, direction, out View))
                //{
                //    if (View.transform.name == "Target")
                //    {
                //        currentDistance = View.distance;
                //        Brain.AddFitness(Math.Abs(lastDistance - currentDistance));
                //    }
                //}
            }


            lastDistance = currentDistance;

        }

        public void GivePurpose(NeuralNetwork brain, Transform target)
        {
            Brain = brain;
            Target = target;
            Alive = true;
        }
    }
}