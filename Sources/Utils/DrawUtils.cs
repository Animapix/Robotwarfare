using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Services;
using System;

public static class DrawUtils
{
    public static void Line(Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
    {
        var distance = Vector2.Distance(point1, point2);
        var angle = MathF.Atan2(point2.Y - point1.Y, point2.X - point1.X);
        Line(point1, distance, angle, color, thickness);
    }

    public static void Line(Vector2 point, float length, float angle, Color color, float thickness = 1f)
    {
        SpriteBatch spriteBatch = Services.ServiceLocator.GetService<SpriteBatch>();
        var origin = new Vector2(0f, 0.5f);
        var scale = new Vector2(length, thickness);
        spriteBatch.Draw(ServiceLocator.GetService<AssetsManager>().GetAsset<Texture2D>("OnePixel"), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
    }

    public static void Rectangle(Vector2 position, float width, float height, Color color, float thickness = 1f)
    {
        Line(position - new Vector2(thickness / 2,0), position + new Vector2(width, 0) + new Vector2(thickness / 2, 0), color, thickness);
        Line(position + new Vector2(width, 0), position + new Vector2(width, height), color, thickness);
        Line(position + new Vector2(width, height) + new Vector2(thickness / 2, 0), position + new Vector2(0, height) - new Vector2(thickness / 2, 0), color, thickness);
        Line(position + new Vector2(0, height), position, color, thickness);
    }

    public static void FillRectangle(Vector2 position, float width, float height, Color color)
    {
        SpriteBatch spriteBatch = Services.ServiceLocator.GetService<SpriteBatch>();
        var origin = new Vector2(0f, 0f);
        var scale = new Vector2(width, height);
        spriteBatch.Draw(ServiceLocator.GetService<AssetsManager>().GetAsset<Texture2D>("OnePixel"), position, null, color, 0, origin, scale, SpriteEffects.None, 0);
    }

    public static void Circle(Vector2 position, float radius, int segments, Color color)
    {
        if (radius <= 0.0f || segments <= 0) return;
        float angleStep = (360.0f / segments);
        angleStep *= ((float)Math.PI * 2) / 360;

        Vector2 lineStart = Vector2.Zero;
        Vector2 lineEnd = Vector2.Zero;

        for (int i = 0; i < segments; i++)
        {
            lineStart.X = MathF.Cos(angleStep * i);
            lineStart.Y = MathF.Sin(angleStep * i);
            lineEnd.X = MathF.Cos(angleStep * (i + 1));
            lineEnd.Y = MathF.Sin(angleStep * (i + 1));
            lineStart *= radius;
            lineEnd *= radius;
            lineStart += position;
            lineEnd += position;
            Line(lineStart, lineEnd, color);
        }
    }

    [Flags]
    public enum Alignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

    public static void Text(SpriteFont font, string text, Rectangle bounds, Alignment align, Color color)
    {
        Vector2 size = font.MeasureString(text);
        Vector2 pos = bounds.Center.ToVector2();
        Vector2 origin = size * 0.5f;

        if (align.HasFlag(Alignment.Left))
            origin.X += bounds.Width / 2 - size.X / 2;

        if (align.HasFlag(Alignment.Right))
            origin.X -= bounds.Width / 2 - size.X / 2;

        if (align.HasFlag(Alignment.Top))
            origin.Y += bounds.Height / 2 - size.Y / 2;

        if (align.HasFlag(Alignment.Bottom))
            origin.Y -= bounds.Height / 2 - size.Y / 2;

        SpriteBatch spriteBatch = Services.ServiceLocator.GetService<SpriteBatch>();
        spriteBatch.DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
    }
}
