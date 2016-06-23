using UnityEngine;
using System.Collections;


namespace MassSpring
{
    public class ParticleBone : MonoBehaviour
    {
        public bool movable = true; // can the particle move or not ? used to pin parts of the cloth

        public float mass = 1; // the mass of the particle (is always 1 in this example)
        public Vector3 pos = Vector3.zero; // the current position of the particle in 3D space
        public Vector3 old_pos = Vector3.zero; // the position of the particle in the previous time step, used as part of the verlet numerical integration scheme
        public Vector3 acceleration = Vector3.zero; // a vector representing the current acceleration of the particle
        public Vector3 force = Vector3.zero;
        void Start()
        {
            old_pos = transform.position;
        }

        void AddForce(Vector3 f)
        {
            force += f;
        }

        public void makeUnmovable()
        {
            movable = false;
        }

        public void timeStep()
        {
            if (movable)
            {
                Vector3 temp = transform.position;
                transform.position = temp + (temp - old_pos) * (1.0f - 0.01f) + Time.deltaTime * force / mass;
                old_pos = temp;
            }

        }

        public void offsetPos(Vector3 v )
        {
            if (movable)
            {
                transform.position += v;
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }


}

