using System;
using System.Collections.Generic;
using System.Numerics;

namespace PojgsaParticleSimulator
{
    public class SimulationGrid
    {
        private float cellSize;
        private int cols;
        private int rows;
        private List<Particle>[,] cells;
        private int width;
        private int height;

        public SimulationGrid(int width, int height, float cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.cols = (int)Math.Ceiling(width / cellSize);
            this.rows = (int)Math.Ceiling(height / cellSize);
            
            cells = new List<Particle>[cols, rows];
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    cells[x, y] = new List<Particle>();
                }
            }
        }

        public void Clear()
        {
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    cells[x, y].Clear();
                }
            }
        }

        public void AddParticle(Particle p)
        {
            int cx = (int)(p.Position.X / cellSize);
            int cy = (int)(p.Position.Y / cellSize);

            // Clamp to grid
            if (cx < 0) cx = 0;
            if (cx >= cols) cx = cols - 1;
            if (cy < 0) cy = 0;
            if (cy >= rows) cy = rows - 1;

            cells[cx, cy].Add(p);
        }

        public void HandleCollisions()
        {
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    HandleCell(x, y);
                }
            }
        }

        private void HandleCell(int x, int y)
        {
            var cellParticles = cells[x, y];
            
            // Check collisions within the cell
            for (int i = 0; i < cellParticles.Count; i++)
            {
                Particle p1 = cellParticles[i];

                // Check against other particles in this cell
                for (int j = i + 1; j < cellParticles.Count; j++)
                {
                    ResolveCollision(p1, cellParticles[j]);
                }

                // Check neighbors (to handle boundary crossing)
                // We only need to check 4 neighbors to avoid double counting (e.g., Right, Bottom-Right, Bottom, Bottom-Left)
                CheckNeighbor(p1, x + 1, y);
                CheckNeighbor(p1, x - 1, y + 1);
                CheckNeighbor(p1, x, y + 1);
                CheckNeighbor(p1, x + 1, y + 1);
            }
        }

        private void CheckNeighbor(Particle p1, int nx, int ny)
        {
            if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
            {
                var neighborParticles = cells[nx, ny];
                foreach (var p2 in neighborParticles)
                {
                    ResolveCollision(p1, p2);
                }
            }
        }

        private void ResolveCollision(Particle p1, Particle p2)
        {
            Vector2 delta = p1.Position - p2.Position;
            float distSq = delta.LengthSquared();
            float radiusSum = p1.Radius + p2.Radius;

            if (distSq < radiusSum * radiusSum && distSq > 0)
            {
                float distance = (float)Math.Sqrt(distSq);
                Vector2 normal = delta / distance;
                float overlap = radiusSum - distance;

                // Separate particles
                float totalInverseMass = p1.InverseMass + p2.InverseMass;
                if (totalInverseMass <= 0) return;

                Vector2 separation = normal * (overlap * 0.5f); // Even split for stability, or use mass ratio
                
                p1.Position += separation * (p1.InverseMass / totalInverseMass);
                p2.Position -= separation * (p2.InverseMass / totalInverseMass);

                // Impulse resolution
                Vector2 relativeVel = p1.Velocity - p2.Velocity;
                float velAlongNormal = Vector2.Dot(relativeVel, normal);

                if (velAlongNormal > 0) return; // Moving apart

                float j = -(1 + Particle.Restitution) * velAlongNormal;
                j /= totalInverseMass;

                Vector2 impulse = j * normal;
                p1.Velocity += impulse * p1.InverseMass;
                p2.Velocity -= impulse * p2.InverseMass;
            }
        }
    }
}

