
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

namespace JKress.AITrainer
{
    public class WalkerAgent : Agent
    {
        /// <summary>
        /// Based on walker example.
        /// Changed body parts and joints for Robot Kyle rig.
        /// Added Heuristic function to test joints by user input.
        /// Added ray perception sensor 3d to help navigate around walls.
        /// </summary>
        /// 
        [Header("Training Type")] //If true, agent is penalized for moving away from target
        public bool earlyTraining = false;

        [Header("Body Parts")]
        [SerializeField] Transform hips;
        [SerializeField] Transform spine;
        [SerializeField] Transform head;
        [SerializeField] Transform thighL;
        [SerializeField] Transform shinL;
        [SerializeField] Transform footL;
        [SerializeField] Transform thighR;
        [SerializeField] Transform shinR;
        [SerializeField] Transform footR;
        [SerializeField] Transform armL;
        [SerializeField] Transform forearmL;
        [SerializeField] Transform armR;
        [SerializeField] Transform forearmR;

        public float hipRotation;

        private float maxAngle = 5;
        private float maxVelocity = 0.3f;
        private float maxAngularVelocity = 0.3f;
        private float maxHead = 1.0f;
        //Should the agent sample a new goal velocity each episode?
        //If true, walkSpeed will be randomly set between zero and m_maxWalkingSpeed in OnEpisodeBegin()
        //If false, the goal velocity will be walkingSpeed
        public bool randomizeWalkSpeedEachEpisode;

        //This will be used as a stabilized model space reference point for observations
        //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
        OrientationCubeController m_OrientationCube;

        //The indicator graphic gameobject that points towards the target
        JointDriveController m_JdController;


        public override void Initialize()
        {
            m_OrientationCube = GetComponentInChildren<OrientationCubeController>();

            //Setup each body part
            m_JdController = GetComponent<JointDriveController>();

            m_JdController.SetupBodyPart(hips);
            m_JdController.SetupBodyPart(spine);
            m_JdController.SetupBodyPart(head);
            m_JdController.SetupBodyPart(thighL);
            m_JdController.SetupBodyPart(shinL);
            m_JdController.SetupBodyPart(footL);
            m_JdController.SetupBodyPart(thighR);
            m_JdController.SetupBodyPart(shinR);
            m_JdController.SetupBodyPart(footR);
            m_JdController.SetupBodyPart(armL);
            m_JdController.SetupBodyPart(forearmL);
            m_JdController.SetupBodyPart(armR);
            m_JdController.SetupBodyPart(forearmR);
        }

        /// <summary>
        /// Loop over body parts and reset them to initial conditions.
        /// </summary>
        public override void OnEpisodeBegin()
        {
            //Reset all of the body parts
            foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
            {
                bodyPart.Reset(bodyPart);
            }

            //Random start rotation to help generalize
            hips.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
        }

        void FixedUpdate()
        {
            AddRewards();
        }

        /// <summary>
        /// Add relevant information on each body part to observations.
        /// </summary>
        public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
        {
            //Get positions
            sensor.AddObservation(bp.rb.transform.localPosition);

            //Get rotations (including hips)
            sensor.AddObservation(bp.rb.transform.localRotation);

            //Get velocities
            //sensor.AddObservation(bp.rb.transform.InverseTransformDirection(bp.rb.velocity));
            sensor.AddObservation(bp.rb.transform.InverseTransformDirection(bp.rb.angularVelocity));

            //Get position relative to hips
            sensor.AddObservation(bp.rb.transform.localPosition - hips.localPosition);
        }

        /// <summary>
        /// Loop over body parts to add them to observation.
        /// </summary>
        public override void CollectObservations(VectorSensor sensor)
        {
            foreach (var bodyPart in m_JdController.bodyPartsList)
            {
                if (bodyPart.rb.tag != "Pelvis")
                {
                    CollectObservationBodyPart(bodyPart, sensor);
                }
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            var bpDict = m_JdController.bodyPartsDict;

            var continuousActions = actionBuffers.ContinuousActions;
            var i = -1;

            bpDict[spine].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]); 
           
            bpDict[thighL].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[thighR].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[shinL].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[shinR].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[footR].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[footL].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);

            bpDict[armL].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[armR].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[forearmL].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[forearmR].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);
        }

        void AddRewards()
        {
            //Stability Rewards
            float uprightReward = CalculateUprightReward();
            AddReward(uprightReward);

            //Positional Rewards
            float positionReward = CalculatePositionReward();
            AddReward(positionReward);

            //Balance Rewards
            float balanceReward = CalculateBalanceReward();
            AddReward(balanceReward);

            //Foot Twist and distance Penalty
            float footTwistPenalty = CalculateFootTwistPenalty();
            AddReward(footTwistPenalty);
            //float footDistanceReward = CalculateFootDistanceReward();
            //AddReward(footDistanceReward);

            //Penalty for Falling
            if (HasFallen())
            {
                AddReward(-1.0f);
                EndEpisode();
            }

            //Time-based rewards
            AddReward(0.01f);
        }

        float CalculateUprightReward()
        {
            float reward = 0f;

            //float angle = Vector3.Angle(spine.transform.up, Vector3.up);
            float angle = Mathf.Abs(spine.transform.localRotation.eulerAngles.x);
            //if (Mathf.Abs(angle) < 2) reward += 0.1f;
            reward += Mathf.Min(1 / angle, 1);

            return reward;
        }

        float CalculatePositionReward()
        {
            float reward = 0f;

            var bpDict = m_JdController.bodyPartsDict;

            float headDiff = bpDict[head].startingPos.y - head.position.y;
            if (headDiff> 0)
                reward += -Mathf.Log10(headDiff + 1) + 0.1f;

            float spineDiff = bpDict[spine].startingPos.y - spine.position.y;
            if (spineDiff> 0)
                reward += -Mathf.Log10(spineDiff + 1) + 0.1f;

            float hipDiff = bpDict[hips].startingPos.y - hips.position.y;
            if (hipDiff > 0)
                reward += -Mathf.Log10(hipDiff + 1) + 0.1f;

            float legDiff = bpDict[thighR].startingPos.y - thighR.position.y;
            if(legDiff > 0)
                reward += -Mathf.Log10(legDiff + 1) + 0.1f;

            return reward;
        }

        float CalculateBalanceReward()
        {
            float reward = 0f;

            var bpDict = m_JdController.bodyPartsDict;

            // head
            float angularVelocity = bpDict[head].rb.angularVelocity.magnitude;
            reward += Mathf.Min(1 / angularVelocity, 1);

            // spine
            angularVelocity = bpDict[spine].rb.angularVelocity.magnitude;
            reward += Mathf.Min(1 / angularVelocity, 1);

            // arm
            angularVelocity = bpDict[armL].rb.angularVelocity.magnitude;
            reward += Mathf.Min(1 / angularVelocity, 1);
            angularVelocity = bpDict[armR].rb.angularVelocity.magnitude;
            reward += Mathf.Min(1 / angularVelocity, 1);

            //forearm
            angularVelocity = bpDict[forearmL].rb.angularVelocity.magnitude;
            reward += Mathf.Min(1 / angularVelocity, 1);
            angularVelocity = bpDict[forearmR].rb.angularVelocity.magnitude;
            reward += Mathf.Min(1 / angularVelocity, 1);

            return reward;
        }

        float CalculateFootTwistPenalty()
        {
            float reward = 0f;

            reward = Vector3.Dot(footR.position - footL.position, -footL.right);

            if (reward > 0.1f) reward = 0.1f;

            return reward;
        }

        float CalculateFootDistanceReward()
        {
            float reward = 0f;

            float distance = Vector3.Distance(footR.position, footL.position);
            reward = -Mathf.Pow(distance - 0.3f, 2) + 0.1f;

            return reward;
        }

        bool HasFallen()
        {
            var bpDict = m_JdController.bodyPartsDict;
            return spine.position.y < bpDict[spine].startingPos.y / 2f;
        }

        //Returns the average velocity of all of the body parts
        //Using the velocity of the hips only has shown to result in more erratic movement from the limbs, so...
        //...using the average helps prevent this erratic movement
        Vector3 GetAvgVelocity()
        {
            Vector3 velSum = Vector3.zero;

            //All Rbs
            int numOfRb = 0;
            foreach (var item in m_JdController.bodyPartsList)
            {
                numOfRb++;
                velSum += item.rb.velocity;
            }

            var avgVel = velSum / numOfRb;
            return avgVel;
        }

        Vector3 GetAvgPosition()
        {
            Vector3 posSum = Vector3.zero;

            //All Rbs
            int numOfRb = 0;
            foreach (var item in m_JdController.bodyPartsList)
            {
                numOfRb++;
                posSum += item.rb.position;
            }

            var avgPos = posSum / numOfRb;
            return avgPos;
        }
    }
}