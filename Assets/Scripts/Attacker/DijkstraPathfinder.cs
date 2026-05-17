using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    public static class DijkstraPathfinder
    {
        private const float GRID_SIZE = 0.25f;

        public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos, LayerMask obstacleLayer)
        {
            Vector2 startGrid = SnapToGrid(startPos);
            Vector2 targetGrid = SnapToGrid(targetPos);

            if (startGrid == targetGrid) return new List<Vector2> { targetGrid };

            Dictionary<Vector2, float> distances = new Dictionary<Vector2, float>();
            Dictionary<Vector2, Vector2> previous = new Dictionary<Vector2, Vector2>();
            List<Vector2> unvisited = new List<Vector2>();

            distances[startGrid] = 0;
            unvisited.Add(startGrid);

            int maxIterations = 500;
            int currentIteration = 0;

            while (unvisited.Count > 0 && currentIteration < maxIterations)
            {
                currentIteration++;
                unvisited.Sort((a, b) => distances.GetValueOrDefault(a, float.MaxValue).CompareTo(distances.GetValueOrDefault(b, float.MaxValue)));
                Vector2 current = unvisited[0];
                unvisited.RemoveAt(0);

                if (Vector2.Distance(current, targetGrid) < GRID_SIZE)
                {
                    targetGrid = current;
                    break;
                }

                foreach (Vector2 neighbor in GetNeighbors(current, obstacleLayer))
                {
                    float alt = distances[current] + Vector2.Distance(current, neighbor);
                    if (!distances.ContainsKey(neighbor) || alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = current;
                        if (!unvisited.Contains(neighbor))
                            unvisited.Add(neighbor);
                    }
                }
            }

            List<Vector2> path = new List<Vector2>();
            Vector2 curr = targetGrid;
            while (previous.ContainsKey(curr))
            {
                path.Insert(0, curr);
                curr = previous[curr];
            }
            
            return path;
        }

        private static Vector2 SnapToGrid(Vector2 pos)
        {
            return new Vector2(
                Mathf.Round(pos.x / GRID_SIZE) * GRID_SIZE,
                Mathf.Round(pos.y / GRID_SIZE) * GRID_SIZE
            );
        }

        private static List<Vector2> GetNeighbors(Vector2 current, LayerMask obstacleLayer)
        {
            List<Vector2> neighbors = new List<Vector2>
            {
                current + new Vector2(GRID_SIZE, 0),
                current + new Vector2(-GRID_SIZE, 0),
                current + new Vector2(0, GRID_SIZE),
                current + new Vector2(0, -GRID_SIZE),
                
                // Adding diagonals can also help smooth out pathing
                current + new Vector2(GRID_SIZE, GRID_SIZE),
                current + new Vector2(-GRID_SIZE, GRID_SIZE),
                current + new Vector2(GRID_SIZE, -GRID_SIZE),
                current + new Vector2(-GRID_SIZE, -GRID_SIZE)
            };

            // Using a larger overlap box to ensure clearance for the agent's collider
            Vector2 checkSize = new Vector2(GRID_SIZE, GRID_SIZE);
            
            return neighbors.Where(n => !Physics2D.OverlapBox(n, checkSize, 0, obstacleLayer)).ToList();
        }
    }
}
