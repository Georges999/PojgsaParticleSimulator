using System;

namespace PojgsaParticleSimulator
{
    public class Particle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Radius { get; set; }
        public float Mass { get; set; } // Mass of the particle
        public float Lifetime { get; private set; }
        public bool IsAlive => Lifetime > 0; // Check if particle is alive
        private static float Gravity = 9.81f; // Gravity constant
        private static readonly float Damping = 0.99f; // Damping factor for energy loss
        private static readonly float Restitution = 0.8f; // Coefficient of restitution                             
        public static float WindIntensity { get; set; } = 0.0f; // Wind intensity
        public static float WindDirection { get; set; } = 0.0f; // Wind direction in radians (0 = right, π/2 = up, etc.)

        public static bool ShowWindVector { get; set; } = true; // Toggle for wind vector visibility
        public static bool ShowVelocityVector { get; set; } = true; // Toggle for velocity vector visibility
        public static bool ShowGravityVector { get; set; } = true; // Toggle for gravity vector visibility


        public Particle(float x, float y, float velocityX, float velocityY, float sizeFactor, float lifetime)
        {
            X = x;
            Y = y;
            VelocityX = velocityX;
            VelocityY = velocityY;

            // Set radius based on size factor (sizeFactor can be a value between 0.5 to 2.0 for example)
            Radius = sizeFactor * 10; // Adjust 10 to scale the size appropriately
            Mass = Radius; // Assuming mass is proportional to the size (volume)
            Lifetime = lifetime;
        }

        public static float GetGravity()
        {
            return Gravity;
        }

        public static void SetGravity(float newGravity)
        {
            Gravity = newGravity;
        }
        public static void SetWind(float intensity, float direction)
        {
            WindIntensity = intensity;
            WindDirection = direction;
        }

        public static float GetWindIntensity()
        {
            return WindIntensity;
        }

        public static float GetWindDirection()
        {
            return WindDirection;
        }

        public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            if (!IsAlive) return; // Update only if the particle is alive

            // Apply gravity
            VelocityY += Gravity * deltaTime;

            // Apply wind effect
            VelocityX += WindIntensity * (float)Math.Cos(WindDirection) * deltaTime;
            VelocityY += WindIntensity * (float)Math.Sin(WindDirection) * deltaTime;

            // Update position
            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;

            // Apply damping to simulate energy loss
            VelocityX *= Damping;
            VelocityY *= Damping;

            // Handle boundary collisions
            if (X - Radius < 0) // Left boundary
            {
                X = Radius; // Position at boundary
                VelocityX = -VelocityX * Restitution; // Reverse horizontal velocity with restitution
            }
            else if (X + Radius > screenWidth) // Right boundary
            {
                X = screenWidth - Radius; // Position at boundary
                VelocityX = -VelocityX * Restitution; // Reverse horizontal velocity with restitution
            }

            if (Y - Radius < 0) // Top boundary
            {
                Y = Radius; // Position at boundary
                VelocityY = -VelocityY * Restitution; // Reverse vertical velocity with restitution
            }
            else if (Y + Radius > screenHeight) // Bottom boundary
            {
                Y = screenHeight - Radius; // Position at boundary
                VelocityY = -VelocityY * Restitution; // Reverse vertical velocity with restitution
            }

            // Update lifetime
            Lifetime -= deltaTime;
        }

        public bool CheckCollision(Particle other)
        {
            float deltaX = X - other.X;
            float deltaY = Y - other.Y;
            float distanceSquared = deltaX * deltaX + deltaY * deltaY;
            float combinedRadius = Radius + other.Radius;

            return distanceSquared < (combinedRadius * combinedRadius);
        }

        public void ResolveCollision(Particle other)
        {
            // Calculate the normal vector
            float deltaX = X - other.X;
            float deltaY = Y - other.Y;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            if (distance == 0) return; // Prevent division by zero

            // Normalize the normal vector
            float normalX = deltaX / distance;
            float normalY = deltaY / distance;

            // Calculate relative velocity
            float relativeVelocityX = VelocityX - other.VelocityX;
            float relativeVelocityY = VelocityY - other.VelocityY;

            // Calculate the velocity along the normal
            float velocityAlongNormal = relativeVelocityX * normalX + relativeVelocityY * normalY;

            // Only resolve if the particles are moving towards each other
            if (velocityAlongNormal > 0) return;

            // Calculate impulse scalar based on mass
            float impulseScalar = -(1 + Restitution) * velocityAlongNormal / (1 / Mass + 1 / other.Mass);

            // Update velocities based on the impulse
            VelocityX += impulseScalar * normalX / Mass;
            VelocityY += impulseScalar * normalY / Mass;
            other.VelocityX -= impulseScalar * normalX / other.Mass;
            other.VelocityY -= impulseScalar * normalY / other.Mass;

            // Separate the particles based on their radii
            float overlap = (Radius + other.Radius) - distance;
            if (overlap > 0)
            {
                // Move particles apart based on their overlap
                float totalRadius = Radius + other.Radius;
                float percent = overlap / totalRadius;

                // Move them apart based on their masses
                X += normalX * percent * other.Radius;
                Y += normalY * percent * other.Radius;

                other.X -= normalX * percent * Radius;
                other.Y -= normalY * percent * Radius;
            }
        }
    }
}
