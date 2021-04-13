#pragma warning disable CS0661 // O tipo define os operadores == ou !=, mas não substitui o Object.GetHashCode()
#pragma warning disable CS0659 // O tipo substitui Object. Equals (objeto o), mas não substitui o Object.GetHashCode()
#define DEVELOPMENT

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vector
    {
        internal float x;
        internal float y;

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector(float angle)
        {
            x = (float)Math.Sin(angle * Math.PI / 180);
            y = (float)Math.Cos(angle * Math.PI / 180);
        }

        public float Angle
        {
            get
            {
                if (Module == 0)
                    return 0;
                return 180 * (float)Math.Asin(X / Module) / (float)Math.PI;
            }
            set
            {
                float delta = value - this.Angle;
                this *= new Vector(delta);
            }
        }

        public float Module
        {
            get => (float)Math.Sqrt(X * X + Y * Y);
            set
            {
                float delta = value / Module;
                this *= delta;
            }
        }

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public Vector Position
        {
            get => this;
            set => this = value;
        }
        public float Rotation
        {
            get => Angle;
            set => Angle = this.Angle;
        }
        public static Vector Origin { get => new Vector(0, 0); }
        public static Vector FromAngle(float angle) => new Vector(angle);
        public static Vector FromAngle(float angle, float module)
        {
            float x = (float)Math.Sin(angle * Math.PI / 180);
            float y = (float)Math.Cos(angle * Math.PI / 180);
            return new Vector(module * x, module * y);
        }
        public float GetDistance(Vector vector)
        {
            float dx = vector.X - X;
            float dy = vector.Y - Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public override string ToString()
        {
            return $"<{this.X}, {this.Y}>";
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
            {
                return false;
            }
            bool xeq = x.Equals(((Vector)obj).x);
            bool yeq = y.Equals(((Vector)obj).y);
            if (xeq && yeq)
                return true;
            else
                return false;
        }

        public static Vector[] GetPolygon (Vector center, float radius, int edges)
        {
            if (edges == 0)
                throw new ArgumentException("\"edges\" cannot be 0.", "edges");

            Vector[] result = new Vector[edges];
            for (int i = 0; i < edges; i++)
                result[i] = center + radius * new Vector(i * 360 / edges);
            return result;
        }

        public static Vector operator +(Vector left, Vector right) =>
            new Vector(
                left.X + right.X,
                left.Y + right.Y);
        public static Vector operator -(Vector left, Vector right) =>
            new Vector(
                left.X - right.X,
                left.Y - right.Y);
        public static Vector operator *(Vector left, Vector right) =>
            new Vector(
                left.X * right.X - left.Y * right.Y,
                left.X * right.Y + left.Y + right.X);
        public static Vector operator *(float scalar, Vector vector) =>
            new Vector(
                vector.X * scalar,
                vector.Y * scalar);
        public static Vector operator *(Vector vector, float scalar) =>
            scalar * vector;
        public static Vector operator /(Vector vector, float scalar) =>
            new Vector(
                vector.X / scalar,
                vector.Y / scalar);

        /*public static bool operator ==(Vector left, Vector right)
        {
            bool xeq = left.x == right.x;
            bool yeq = left.y == right.y;
            return xeq && yeq;
        }
        public static bool operator !=(Vector left, Vector right)
        {
            bool xdif = left.x != right.x;
            bool ydif = left.y != right.y;
            return xdif || ydif;
        }*/
    }
}
