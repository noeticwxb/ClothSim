using UnityEngine;
using System.Collections;

namespace MassSpring
{
    public class Constraint
    {
        float rest_distance;
        ParticleBone p1, p2;
        public Constraint(ParticleBone _p1, ParticleBone _p2)

        {
            p1 = _p1;
            p2 = _p2;

            rest_distance = (p2.transform.position - p1.transform.position).sqrMagnitude;
        }

        public void satisfyConstraint()
        {
            Vector3 p1_to_p2 = p2.transform.position - p1.transform.position; // vector from p1 to p2
            float current_distance = p1_to_p2.sqrMagnitude; // current distance between p1 and p2

            Vector3 correctionVector = p1_to_p2 * (1 - rest_distance / current_distance); // The offset vector that could moves p1 into a distance of rest_distance to p2

            Vector3 correctionVectorHalf = correctionVector * 0.5f; // Lets make it half that length, so that we can move BOTH p1 and p2.

            p1.offsetPos(correctionVectorHalf); // correctionVectorHalf is pointing from p1 to p2, so the length should move p1 half the length needed to satisfy the constraint.
            p2.offsetPos(-correctionVectorHalf); // we must move p2 the negative direction of correctionVectorHalf since it points from p2 to p1, and not p1 to p2.	

        }
    }
}

