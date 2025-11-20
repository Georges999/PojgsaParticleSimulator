using System;
using System.Drawing;
using System.Numerics;

namespace PojgsaParticleSimulator
{
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        
        public float Radius { get; set; }
        public float Mass { get; set; }
        public float InverseMass { get; private set; }
        public float Lifetime { get; set; }
        public float MaxLifetime { get; private set; }
        public Color Color { get; set; }

        public bool IsAlive => Lifetime > 0;

        // Physics Constants (Global)
        public static float Gravity = 9.81f;
        public static float Damping = 0.995f; // Slightly less damping for more "floaty" feel
        public static float Restitution = 0.7f;
        public static Vector2 Wind = Vector2.Zero;

        public static bool ShowWindVector { get; set; } = false;
        public static bool ShowVelocityVector { get; set; } = false;
        public static bool ShowGravityVector { get; set; } = false;

        public Particle(Vector2 position, Vector2 velocity, float radius, float lifetime, Color color)
        {
            Position = position;
            Velocity = velocity;
            Radius = radius;
            Mass = radius; // Mass proportional to radius (2D approximation)
            InverseMass = Mass > 0 ? 1.0f / Mass : 0;
            Lifetime = lifetime;
            MaxLifetime = lifetime;
            Color = color;
            Acceleration = Vector2.Zero;
        }

        public void ApplyForce(Vector2 force)
        {
            if (InverseMass == 0) return;
            Acceleration += force * InverseMass;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            if (!IsAlive) return;

            // Apply Gravity
            ApplyForce(new Vector2(0, Gravity * Mass * 50)); // Scale gravity for visual effect

            // Apply Wind
            ApplyForce(Wind * Mass); // Wind force

            // Verlet/Euler Integration
            Velocity += Acceleration * deltaTime;
            Velocity *= Damping;
            Position += Velocity * deltaTime;

            // Reset Acceleration
            Acceleration = Vector2.Zero;

            // Boundary Checks
            HandleBoundaries(screenWidth, screenHeight);

            // Decrease Lifetime
            Lifetime -= deltaTime;
        }

        private void HandleBoundaries(int width, int height)
        {
            if (Position.X - Radius < 0)
            {
                Position.X = Radius;
                Velocity.X = -Velocity.X * Restitution;
            }
            else if (Position.X + Radius > width)
            {
                Position.X = width - Radius;
                Velocity.X = -Velocity.X * Restitution;
            }

            if (Position.Y - Radius < 0)
            {
                Position.Y = Radius;
                Velocity.Y = -Velocity.Y * Restitution;
            }
            else if (Position.Y + Radius > height)
            {
                Position.Y = height - Radius;
                Velocity.Y = -Velocity.Y * Restitution;
            }
        }

        public static void SetGravity(float gravity)
        {
            Gravity = gravity;
        }

        public static void SetWind(float intensity, float directionRadians)
        {
            Wind = new Vector2(
                intensity * (float)Math.Cos(directionRadians),
                intensity * (float)Math.Sin(directionRadians)
            );
        }
    }
}

