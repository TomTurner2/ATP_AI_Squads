﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class TacticalAssessor : MonoBehaviour
{
    [SerializeField] List<WeightModule> weight_modules = new List<WeightModule>();
    [SerializeField] bool debug = false;
    [SerializeField] List<WeightedPoint> debug_last_sample = new List<WeightedPoint>();

    private const int POSITION_START_WEIGHT = 100;

    [Serializable]
    public struct WeightedPoint
    {
        public Vector3 position;
        public Vector3 original_position;
        public int weight;
    }


    public List<Vector3> FindOptimalCoverInArea(Vector3 _position, float _radius, Faction _requesters_faction, int _sample_count = 30,
        int _minimum_score = 0, int _area_mask = NavMesh.AllAreas)
    {
        List<WeightedPoint> weighted_positions = SampleDistributedPointInRadius(_position,
            _radius, _area_mask);//select random points on nav

        if (weighted_positions.Count <= 0)//if none sampled leave
            return new List<Vector3>();

        FindCoverLocations(ref weighted_positions, _position, _radius, _requesters_faction,
            _sample_count, _minimum_score, _area_mask);//find cover

        weighted_positions = new List<WeightedPoint>(weighted_positions.OrderByDescending(p => p.weight));
        List<Vector3> positions = new List<Vector3>(weighted_positions.Select(p => p.position));
        debug_last_sample = weighted_positions;

        return positions;
    }


    private void FindCoverLocations(ref List<WeightedPoint> _weighted_positions, Vector3 _position, float _radius, Faction _requesters_faction,
        int _sample_count = 30, int _minimum_score = 0, int _area_mask = NavMesh.AllAreas)
    {
        for (int i = 0; i < _weighted_positions.Count; ++i)
        {
            WeightedPoint point = _weighted_positions[i];
            NavMeshHit hit;

            if (!NavMesh.FindClosestEdge(point.position, out hit, _area_mask))//get closest edge on nav mesh
                continue;
           
            point.position = hit.position;//set it as position
            point.weight = DetermineWeight(point, _position, _radius, _requesters_faction, ref _weighted_positions);//calculate weighting
            _weighted_positions[i] = point;    
        }
    }


    private List<WeightedPoint> SampleDistributedPointInRadius(Vector3 _position, float _radius, int _area_mask = NavMesh.AllAreas)
    {
        List<Vector3> positions = new List<Vector3>();
        List<WeightedPoint> weight_positions = new List<WeightedPoint>();

        positions = CustomMath.DistributedPointsInRadius(_position, _radius);

        NavMeshHit hit;

        foreach (Vector3 position in positions)
        {
            if (!NavMesh.SamplePosition(position + _position, out hit, 100f, _area_mask))//if hit nav mesh
                continue;

            WeightedPoint point = new WeightedPoint//create struct
            {
                original_position = hit.position,
                position = hit.position,
                weight = POSITION_START_WEIGHT
            };

            weight_positions.Add(point);//add position to list
        }

        debug_last_sample = weight_positions;
        return weight_positions;
    }


    private int DetermineWeight(WeightedPoint _point, Vector3 _position, float _radius, Faction _requesters_faction,
        ref List<WeightedPoint> _weighted_positions)
    {
        int weight =  _point.weight;

        foreach (WeightModule module in weight_modules)
        {
            if (module == null)
                continue;

            weight += module.AssessWeight(_point, _radius, ref _weighted_positions, _requesters_faction);
        }

        return  weight;
    }


    private void OnDrawGizmos()
    {
        if (!debug)
            return;

        Gizmos.color = Color.white;

        if (debug_last_sample == null)
            return;

        foreach (WeightedPoint point in debug_last_sample)
        {
            Gizmos.DrawSphere(point.original_position, 0.05f);
        }
    }

}
