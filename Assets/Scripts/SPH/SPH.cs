using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;
using UnityEngine;

public class SPH : MonoBehaviour
{
    /*
    private float gasConstant = 100.0f;
    private float restDensityMin = 0.0f;
    private float restDensityMax = 5.0f;
    private float densityAdaptationTime = 0.1f; // number of seconds over which to compute the average density
    private float viscosity = 0;
    private bool isObstacle = false;

    // Constants used in kernel functions; they can be precomputed as soon as range_ has been set.
    private float POLY_6, SPIKY_GRAD, VISC_LAP;

    private float rangeSquared;
    private float baseDensityContribution;

    static string GetName() { return "SPH"; }
    bool AssumesObstacleParticles() { return isObstacle; }

    void parseParameters(const CostFunctionParameters params) override;

    struct DensityData
    {
        float density;
        float pressure;
        float restDensity;

        public DensityData() { density = 0f; pressure = 0f; restDensity = 0f; }
    };

    float GetRestDensityMin() { return restDensityMin; }
	float GetRestDensityMax() { return restDensityMax; }
	float GetDensityAdaptationTime() { return densityAdaptationTime; }

    const bool UseObstacleParticles_Density = false;
    const bool UseObstacleParticles_PressureForce = false;

    void ComputeDensityData(Agent agent, float dt, DensityData result, DensityData result_progressive)
    {
        const auto neighbors = agent.getNeighbors();
        const Vector2 agentPos = agent.getPosition();

        result.density = 0;
        result.pressure = 0;

        foreach (const auto neighbor in neighbors.first)
        {
            if (!UseObstacleParticles_Density && neighbor.realAgent->isSPHObstacleParticle())
                continue;

            float diff = rangeSquared - neighbor.GetDistanceSquared();
            if (diff > 0)
            {
                result.density += neighbor.realAgent.getMass() * POLY_6 * Mathf.Pow(diff, 3.0f);
            }
        }

        result.density += agent.getMass() * baseDensityContribution;


        // - add neighboring obstacles (but not if this agent itself is a boundary particle)
        if (!UseObstacleParticles_Density && !agent->isSPHObstacleParticle())
        {
            foreach (const auto neighborObs in neighbors.second)
		{
                // compute the area V that this obstacle segment occupies inside the kernel circle
                float obsVolume = getObstacleVolumeInsideCircle(neighborObs, agentPos, range_);
                if (obsVolume > 0)
                {
                    float distanceToObstacle = distanceToLine(agentPos, neighborObs.first, neighborObs.second, true);
                    float distanceToCenterOfMass = (range_ + distanceToObstacle) / 2.0f;
                    // density_b = restDensity * V * W(distance) 
                    float diff = rangeSquared - distanceToCenterOfMass * distanceToCenterOfMass;
                    result.density += obsVolume * result.restDensity * POLY_6 * powf(diff, 3.0f);
                }
            }
        }

        // update the progressive average density over time

        float frac = dt / densityAdaptationTime;
        result_progressive.density = (1 - frac) * result_progressive.density + frac * result.density;

        // compute the current rest density

        result.restDensity = Mathf.Clamp((result_progressive.density, restDensityMin, restDensityMax);

        // compute pressure
        result.pressure = gasConstant * (result.density - result.restDensity);
    }

    Vector2 ComputeAgentInteractionForce(const Agent agent, const PhantomAgent other)
    {

        const DensityData data_i = agent.getSPHDensityData();
        const DensityData data_j = other.realAgent.getSPHDensityData();
        float mass_i = agent.getMass();
        float mass_j = other.realAgent.getMass();

        Vector2 result = new Vector2(0, 0);

	    // if we've chosen to ignore obstacle particles, and this neighbor is an obstacle particle, ignore it
	    if (!UseObstacleParticles_PressureForce && other.realAgent.isSPHObstacleParticle())
		    return result;

	    // if the neighboring agent is too far away, ignore it (this shouldn't happen; distant agents have already been filtered out)
	    if (other.GetDistanceSquared() >= rangeSquared)
		    return result;

	    const float dist = Mathf.Sqrt(other.GetDistanceSquared());
        const float rangeMinDist = range_ - dist;

	    // pressure force
	    if (data_i.pressure > 0)
		    result += mass_j* (data_i.pressure + data_j.pressure) / (2.0f * data_j.density) * SPIKY_GRAD* rangeMinDist * rangeMinDist / dist* (other.GetPosition() - agent.getPosition());
		    //result += mass_j * (data_i.pressure / (data_i.density*data_i.density) + data_j.pressure / (data_j.density*data_j.density)) * SPIKY_GRAD * rangeMinDist * rangeMinDist / dist * (other.GetPosition() - agent->getPosition());

	    // viscosity force
	    if (viscosity > 0)
		    result += viscosity* mass_j * VISC_LAP* rangeMinDist / data_j.density* (other.GetVelocity() - agent.getVelocity());

	    return result / data_i.density;
    }

    Vector2 ComputeObstacleInteractionForce(const Agent agent, const LineSegment2D obstacle, const Vector2D nearest)
    {
	    Vector2 result = new Vector2(0, 0);

        // if we've chosen to use obstacle particles instead, ignore the obstacle itself
        if (UseObstacleParticles_PressureForce)
            return result;

        const Vector2 agentPos = agent.getPosition();
        const DensityData data_i = agent.getSPHDensityData();
        float mass_i = agent.getMass();

        // compute the area V that this obstacle segment occupies inside the kernel circle
        // TODO: maybe store it, so we don't have to compute it twice?
        float obsVolume = getObstacleVolumeInsideCircle(obstacle, agentPos, range_);
        if (obsVolume > 0)
        {
            float distanceToObstacle = distance(agentPos, nearest);
            float distanceToCenterOfMass = (range_ + distanceToObstacle) / 2.0f;
            float rangeMinDist = range_ - distanceToCenterOfMass;

            // pressure force. Assumptions: obstacle pressure == agent pressure, obstacle density == rest density
            if (data_i.pressure > 0)
                result += obsVolume * data_i.pressure * SPIKY_GRAD * rangeMinDist * rangeMinDist / distanceToObstacle * (nearest - agentPos);
            //result += obsVolume * (data_i.pressure / (data_i.density * data_i.density) + (data_i.pressure / (data_i.restDensity * data_i.restDensity))) * SPIKY_GRAD * rangeMinDist * rangeMinDist / dist * (nearest - agentPos);

            // viscosity force is ignored for now; this would make agents stick to walls
        }

        return result / data_i.density;
    }

    float getObstacleVolumeInsideCircle(const LineSegment2D segment, const Vector2 circleCenter, const float circleRadius)
    {
	    // get the part of the obstacle segment that lies inside the circle
	    bool isInsideCircle;
        LineSegment2D part(getObstaclePartInsideCircle(segment, circleCenter, circleRadius, isInsideCircle));
        if (!isInsideCircle)
            return 0;

        // compute the circle section bounded by these two points
        float ang = Mathf.DeltaAngle(part.first - circleCenter, part.second - circleCenter);
        float circleSectionArea = circleRadius * circleRadius * ang / 2;

        // compute the triangle bounded by the circle center and these two points
        float triangleArea = 0.5f * distance(part.first, part.second) * distanceToLine(circleCenter, segment.first, segment.second);

        return circleSectionArea - triangleArea;
    }

    LineSegment2D getObstaclePartInsideCircle(const LineSegment2D segment, const Vector2D circleCenter, const float circleRadius, bool resultIsValid)
    {
	    const Vector2 p = segment.first - circleCenter;
        const float radiusSq = circleRadius * circleRadius;

        const Vector2 v = segment.second - segment.first;

        // To find out at where the line intersects the disk, we must solve the following equation:
        //      dist(pCircle, pObsStart + v2*t) = r
        // =>   || PDiff + v*t ||^2 = r^2
        // =>   V.x^2*t^2 + 2*P.x*V.x*t + P.x^2 + (same for .y) = R^2
        // =>   (V.x*V.x + V.y*V.y) * t^2 + 2*(P.x*V.x + P.y*V.y) * t + (P.x*P.x + P.y*P.y) = R^2
        // =>   (V.V)*t^2 + 2*(P.)*t + (P.P) = R^2

        float t1 = MaxFloat, t2 = MaxFloat;
        const int nrSolutions = SolveQuadraticEquation(Vector2.Dot(v, v), 2 * Vector2.Dot(p, v), Vector2.Dot(p, p) - radiusSq, t1, t2);

        // if there are not 2 solutions, then the obstacle does not intersect the circle (or it only barely touches it),
        // so it does not contribute to any SPH quantities.
        if (nrSolutions != 2)
        {
            resultIsValid = false;
            return LineSegment2D();
        }

        // order the two solutions, and bound them to the endpoints of the initial segment
        float tFirst = Mathf.Max(0.0f, Mathf.Min(t1, t2));
        float tSecond = Mathf.Min(1.0f, Mathf.Max(t1, t2));

        // convert back to points, and return the result
        resultIsValid = true;
        return LineSegment2D(segment.first + tFirst * v, segment.first + tSecond * v);
    }

    void parseParameters(const CostFunctionParameters params)
    {
        parseParameters (params);
	    params.ReadFloat("gasConstant", gasConstant);
	    params.ReadFloat("restDensityMin", restDensityMin);
	    params.ReadFloat("restDensityMax", restDensityMax);
	    params.ReadFloat("densityAdaptationTime", densityAdaptationTime);
	    params.ReadFloat("viscosity", viscosity);
	    params.ReadBool("isObstacle", isObstacle);

        rangeSquared = range_ * range_;

        //POLY_6 = (float)(315.0 / (64*PI*pow(range_, 9))); // 3D version
        POLY_6 = (float)(4.0 / (Mathf.PI * Mathf.Pow(range_, 8)));

        //SPIKY_GRAD = (float)(-45.0 / (PI*pow(range_, 6))); // 3D version
        SPIKY_GRAD = (float)(-30.0 / (Mathf.PI * Mathf.Pow(range_, 5)));

        //VISC_LAP = (float)(45.0 / (PI*pow(range_, 6))); // 3D version
        VISC_LAP = (float)(360.0 / (29 * Mathf.PI * Mathf.Pow(range_, 5)));

        baseDensityContribution = POLY_6 * Mathf.Pow(rangeSquared, 3.0f);
    }
    */
}
