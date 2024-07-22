
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;

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
            hips.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
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
            //Interaction Objects Contact Check
            sensor.AddObservation(bp.objectContact.touchingGround); // Is this bp touching the ground
            sensor.AddObservation(bp.objectContact.touchingWall); // Is this bp touching the wall

            //Get positions
            sensor.AddObservation(bp.rb.position);
            sensor.AddObservation(bp.rb.transform.localPosition);

            //Get rotations (including hips)
            sensor.AddObservation(bp.rb.rotation);
            sensor.AddObservation(bp.rb.transform.localRotation);

            //Get velocities
            sensor.AddObservation(bp.rb.velocity);
            sensor.AddObservation(bp.rb.angularVelocity);

            //Get position relative to hips
            sensor.AddObservation(bp.rb.position - hips.position);
        }

        /// <summary>
        /// Loop over body parts to add them to observation.
        /// </summary>
        public override void CollectObservations(VectorSensor sensor)
        {
            foreach (var bodyPart in m_JdController.bodyPartsList)
            {
                CollectObservationBodyPart(bodyPart, sensor);
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
            bpDict[head].SetTorque(continuousActions[++i], continuousActions[++i], continuousActions[++i], continuousActions[++i]);

            /*
            bpDict[spine].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

            bpDict[thighL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
            bpDict[thighR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
            bpDict[shinL].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[shinR].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[footR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);
            bpDict[footL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

            bpDict[armL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
            bpDict[armR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
            bpDict[forearmL].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[forearmR].SetJointTargetRotation(continuousActions[++i], 0, 0);
            bpDict[head].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

            //update joint strength settings
            bpDict[spine].SetJointStrength(continuousActions[++i]);
            bpDict[head].SetJointStrength(continuousActions[++i]);
            bpDict[thighL].SetJointStrength(continuousActions[++i]);
            bpDict[shinL].SetJointStrength(continuousActions[++i]);
            bpDict[footL].SetJointStrength(continuousActions[++i]);
            bpDict[thighR].SetJointStrength(continuousActions[++i]);
            bpDict[shinR].SetJointStrength(continuousActions[++i]);
            bpDict[footR].SetJointStrength(continuousActions[++i]);
            bpDict[armL].SetJointStrength(continuousActions[++i]);
            bpDict[forearmL].SetJointStrength(continuousActions[++i]);
            bpDict[armR].SetJointStrength(continuousActions[++i]);
            bpDict[forearmR].SetJointStrength(continuousActions[++i]);
            */
        }

        public override void Heuristic(in ActionBuffers actionBuffers)
        {
            float x = Input.GetAxis("Vertical");
            float y = Input.GetAxis("Horizontal");
            float force = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;

            var continuousActions = actionBuffers.ContinuousActions;
            var i = -1;

            //SetJointTargetRotation
            continuousActions[++i] = -x; continuousActions[++i] = y; continuousActions[++i] = y;
            continuousActions[++i] = x; continuousActions[++i] = y;
            continuousActions[++i] = x; continuousActions[++i] = y;
            continuousActions[++i] = x;
            continuousActions[++i] = x;
            continuousActions[++i] = x; continuousActions[++i] = y; continuousActions[++i] = y;
            continuousActions[++i] = x; continuousActions[++i] = y; continuousActions[++i] = y;
            continuousActions[++i] = x; continuousActions[++i] = y;
            continuousActions[++i] = x; continuousActions[++i] = y;
            continuousActions[++i] = x;
            continuousActions[++i] = x;
            continuousActions[++i] = x; continuousActions[++i] = y;

            //SetJointStrength 
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
            continuousActions[++i] = force;
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
            float footDistanceReward = CalculateFootDistanceReward();
            AddReward(footDistanceReward);

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

            float angle = Vector3.Angle(spine.transform.up, Vector3.up);
            if (angle < maxAngle) reward += 0.1f;

            return reward;
        }

        float CalculatePositionReward()
        {
            float reward = 0f;

            var bpDict = m_JdController.bodyPartsDict;

            float headDiff = bpDict[head].startingPos.y - head.position.y;
            if (headDiff < 0.5f)
            {
                reward += Mathf.Exp(-headDiff);
            }

            float spineDiff = bpDict[spine].startingPos.y - spine.position.y;
            if (spineDiff < 0.5f) reward += Mathf.Exp(-spineDiff);

            float hipDiff = bpDict[hips].startingPos.y - hips.position.y;
            if (hipDiff < 0.5f) reward += Mathf.Exp(-hipDiff);

            return reward;
        }

        float CalculateBalanceReward()
        {
            float reward = 0f;

            var bpDict = m_JdController.bodyPartsDict;

            float headDiff = bpDict[head].startingPos.y - head.position.y;
            
            // 머리는 최대한 안흔들리도록
            float headVelocity = bpDict[head].rb.velocity.magnitude;
            if (headVelocity < maxVelocity) reward += 0.1f;
            float heaAngVelocity = bpDict[head].rb.angularVelocity.magnitude;
            if (heaAngVelocity < maxAngularVelocity) reward += 0.1f;

            // 머리가 어느정도 올라가면 그 후로는 안정성에 reward
            if (headDiff < 0.3f)
            {
                // spine
                float velocity = bpDict[spine].rb.velocity.magnitude;
                if (velocity < maxVelocity) reward += 0.1f;
                float angularVelocity = bpDict[spine].rb.angularVelocity.magnitude;
                if (angularVelocity < maxAngularVelocity) reward += 0.1f;

                // arm
                velocity = bpDict[armL].rb.velocity.magnitude;
                if (velocity < maxVelocity) reward += 0.1f;
                angularVelocity = bpDict[armL].rb.angularVelocity.magnitude;
                if (angularVelocity < maxAngularVelocity) reward += 0.1f;
                velocity = bpDict[armR].rb.velocity.magnitude;
                if (velocity < maxVelocity) reward += 0.1f;
                angularVelocity = bpDict[armR].rb.angularVelocity.magnitude;
                if (angularVelocity < maxAngularVelocity) reward += 0.1f;

                //forearm
                velocity = bpDict[forearmL].rb.velocity.magnitude;
                if (velocity < maxVelocity) reward += 0.1f;
                angularVelocity = bpDict[forearmL].rb.angularVelocity.magnitude;
                if (angularVelocity < maxAngularVelocity) reward += 0.1f;
                velocity = bpDict[forearmR].rb.velocity.magnitude;
                if (velocity < maxVelocity) reward += 0.1f;
                angularVelocity = bpDict[forearmR].rb.angularVelocity.magnitude;
                if (angularVelocity < maxAngularVelocity) reward += 0.1f;
            }

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
            reward = -Mathf.Pow(distance - 0.2f, 2) + 0.1f;

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