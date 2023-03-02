using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Services;
using System;
using System.Collections.Generic;

namespace Animapix.TileMap
{
    public class Tile
    {
        private static Texture2D _tileSetTexture;
        private static Dictionary<string, Rectangle> quads = new Dictionary<string, Rectangle>(){
                { "Corner", new Rectangle(0, 0, 16, 16) },
                { "Square", new Rectangle(16, 0, 16, 16) },
                { "InnerCorner", new Rectangle(32, 0, 16, 16) },
                { "CornerOutLine", new Rectangle(48, 0, 16, 16) },
                { "SquareOutLine", new Rectangle(64, 0, 16, 16) },
                { "InnerCornerOutLine", new Rectangle(0, 16, 32, 32) },
                { "CornerShadow", new Rectangle(32, 16, 32, 32) },
                { "SquareShadow", new Rectangle(64, 16, 16, 16) },
                { "InnerCornerShadow", new Rectangle(64, 32, 16, 16) }
        };

        public enum Type { WALL, FLOOR }
        public const int size = 32;

        public Type type;
        public int bitmask = 0;

        public int column { get; private set; }
        public int row { get; private set; }

        public static void SetTilesTexture(Texture2D tex)
        {
            _tileSetTexture = tex;
        }

        public Tile(int column, int row, Type type)
        {
            this.column = column;
            this.row = row;
            this.type = type;
        }

        public void DrawFilling(SpriteBatch spriteBatch,Color wallsColor, Color floorColor, Vector2 offsetPosition)
        {
            Vector2 center = new Vector2(size / 2, size / 2);
            Vector2 position = new Vector2(column * size, row * size) + center + offsetPosition; 

            spriteBatch.Draw(_tileSetTexture, position, quads["Square"], floorColor, 0, center, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(_tileSetTexture, position, quads["Square"], floorColor, MathF.PI / 2, center, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(_tileSetTexture, position, quads["Square"], floorColor, -MathF.PI / 2, center, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(_tileSetTexture, position, quads["Square"], floorColor, MathF.PI, center, 1, SpriteEffects.None, 0);

            if (type == Type.WALL)
            {
                if ((bitmask & 1 << 0) != 0 || (bitmask & 1 << 1) != 0 || (bitmask & 1 << 3) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["Square"], wallsColor, 0, center, 1, SpriteEffects.None, 0);
                else spriteBatch.Draw(_tileSetTexture, position, quads["Corner"], wallsColor, 0, center, 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 2) != 0 || (bitmask & 1 << 1) != 0 || (bitmask & 1 << 4) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["Square"], wallsColor, MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                else spriteBatch.Draw(_tileSetTexture, position, quads["Corner"], wallsColor, MathF.PI / 2, center, 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 3) != 0 || (bitmask & 1 << 5) != 0 || (bitmask & 1 << 6) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["Square"], wallsColor, -MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                else spriteBatch.Draw(_tileSetTexture, position, quads["Corner"], wallsColor, -MathF.PI / 2, center, 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 4) != 0 || (bitmask & 1 << 6) != 0 || (bitmask & 1 << 7) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["Square"], wallsColor, MathF.PI, center, 1, SpriteEffects.None, 0);
                else spriteBatch.Draw(_tileSetTexture, position, quads["Corner"], wallsColor, MathF.PI, center, 1, SpriteEffects.None, 0);
            }
            else if (type == Type.FLOOR)
            {
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 3) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCorner"], wallsColor, 0, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 4) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCorner"], wallsColor, MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 3) != 0 && (bitmask & 1 << 6) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCorner"], wallsColor, -MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 4) != 0 && (bitmask & 1 << 6) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCorner"], wallsColor, MathF.PI, center, 1, SpriteEffects.None, 0);
            }
        }

        public void DrawOutline(SpriteBatch spriteBatch,Color color, Vector2 offsetPosition)
        {
            Vector2 center = new Vector2(size / 2, size / 2);
            Vector2 position = new Vector2(column * size, row * size) + center + offsetPosition;

            if (type == Type.FLOOR)
            {
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 3) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerOutLine"], color, 0, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 4) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerOutLine"], color, MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 3) != 0 && (bitmask & 1 << 6) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerOutLine"], color, -MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 4) != 0 && (bitmask & 1 << 6) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerOutLine"], color, MathF.PI, center, 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 0) != 0 && (bitmask & 1 << 3) != 0 && (bitmask & 1 << 1) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, 0, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 3) != 0 && (bitmask & 1 << 5) != 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, 0, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 0) != 0 && (bitmask & 1 << 1) != 0 && (bitmask & 1 << 3) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, MathF.PI / 2, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 2) != 0 && (bitmask & 1 << 4) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, MathF.PI / 2, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 2) != 0 && (bitmask & 1 << 4) != 0 && (bitmask & 1 << 1) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, MathF.PI, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 4) != 0 && (bitmask & 1 << 7) != 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, MathF.PI, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 5) != 0 && (bitmask & 1 << 6) != 0 && (bitmask & 1 << 3) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, -MathF.PI / 2, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 6) != 0 && (bitmask & 1 << 7) != 0 && (bitmask & 1 << 4) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareOutLine"], color, -MathF.PI / 2, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);


                if ((bitmask & 1 << 7) != 0 && (bitmask & 1 << 4) == 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerOutLine"], color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 5) != 0 && (bitmask & 1 << 3) == 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerOutLine"], color, MathF.PI / 2, Vector2.Zero, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 0) != 0 && (bitmask & 1 << 1) == 0 && (bitmask & 1 << 3) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerOutLine"], color, MathF.PI, Vector2.Zero, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 2) != 0 && (bitmask & 1 << 1) == 0 && (bitmask & 1 << 4) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerOutLine"], color, -MathF.PI / 2, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }

        public void DrawShadow(SpriteBatch spriteBatch,Color color, Vector2 offsetPosition)
        {
            Vector2 center = new Vector2(size / 2, size / 2);
            Vector2 position = new Vector2(column * size, row * size) + center + offsetPosition;
            if (type == Type.FLOOR)
            {
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 3) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerShadow"], color, 0, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 4) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerShadow"], color, MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 3) != 0 && (bitmask & 1 << 6) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerShadow"], color, -MathF.PI / 2, center, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 4) != 0 && (bitmask & 1 << 6) != 0) spriteBatch.Draw(_tileSetTexture, position, quads["InnerCornerShadow"], color, MathF.PI, center, 1, SpriteEffects.None, 0);


                if ((bitmask & 1 << 0) != 0 && (bitmask & 1 << 3) != 0 && (bitmask & 1 << 1) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, 0, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 3) != 0 && (bitmask & 1 << 5) != 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, 0, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 0) != 0 && (bitmask & 1 << 1) != 0 && (bitmask & 1 << 3) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, MathF.PI / 2, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 1) != 0 && (bitmask & 1 << 2) != 0 && (bitmask & 1 << 4) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, MathF.PI / 2, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 2) != 0 && (bitmask & 1 << 4) != 0 && (bitmask & 1 << 1) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, MathF.PI, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 4) != 0 && (bitmask & 1 << 7) != 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, MathF.PI, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);

                if ((bitmask & 1 << 5) != 0 && (bitmask & 1 << 6) != 0 && (bitmask & 1 << 3) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, -MathF.PI / 2, new Vector2(size / 2, size / 2), 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 6) != 0 && (bitmask & 1 << 7) != 0 && (bitmask & 1 << 4) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["SquareShadow"], color, -MathF.PI / 2, new Vector2(size / 2, 0), 1, SpriteEffects.None, 0);



                if ((bitmask & 1 << 7) != 0 && (bitmask & 1 << 4) == 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerShadow"], color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 5) != 0 && (bitmask & 1 << 3) == 0 && (bitmask & 1 << 6) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerShadow"], color, MathF.PI / 2, Vector2.Zero, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 0) != 0 && (bitmask & 1 << 1) == 0 && (bitmask & 1 << 3) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerShadow"], color, MathF.PI, Vector2.Zero, 1, SpriteEffects.None, 0);
                if ((bitmask & 1 << 2) != 0 && (bitmask & 1 << 1) == 0 && (bitmask & 1 << 4) == 0) spriteBatch.Draw(_tileSetTexture, position, quads["CornerShadow"], color, -MathF.PI / 2, Vector2.Zero, 1, SpriteEffects.None, 0);

            }
        }

        public void DrawDebug(SpriteBatch spriteBatch, Color color, Vector2 offsetPosition)
        {
            AssetsService assets = ServiceLocator.GetService<AssetsService>();
            SpriteFont font = assets.GetAsset<SpriteFont>("DefaultFont");
            Vector2 position = new Vector2(column * size, row * size) + offsetPosition;
            DrawUtils.Text(font, bitmask.ToString(), new Rectangle((int)position.X, (int)position.Y, size, size), DrawUtils.Alignment.Center, color);
            DrawUtils.Rectangle(position, size, size, color);
        }

    }

}
