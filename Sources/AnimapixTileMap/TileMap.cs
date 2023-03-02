using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ánimapix.TileMap
{
    public class TileMap
    {
        public int columns { get; private set; }
        public int rows { get; private set; }

        public Tile[][] tiles { get; private set; }
        private static Random _random = new Random();

        public Color wallsColor = new Color(40, 40, 40);
        public Color floorColor = new Color(80, 80, 80);
        public Color outlinesColor = new Color(10, 10, 10);
        public Color shadowsColor = new Color(0, 0, 0);

        public Vector2 position = Vector2.Zero;

        public TileMap(int columns, int rows, Texture2D tileSet)
        {
            Tile.SetTilesTexture(tileSet);

            this.columns = columns;
            this.rows = rows;

            // Init tiles array
            tiles = new Tile[columns][];
            for (int i = 0; i < columns; i++)
            {
                tiles[i] = new Tile[rows];
                for (int j = 0; j < rows; j++)
                    tiles[i][j] = new Tile(i, j, Tile.Type.WALL);
            }
        }

        /// <summary>
        /// This method fill the tile map with 2d array of booleans
        /// </summary>
        public void Fill(bool[][] values)
        {
            if (columns != values.Length && rows != values[0].Length) return;

            for (int column = 0; column < columns; column++)
                for (int row = 0; row < rows; row++)
                    SetTileType(column, row, values[column][row] ? Tile.Type.WALL : Tile.Type.FLOOR);

            RefreshBitmask(0, 0, columns, rows);
        }

        /// <summary>
        /// This method returns the tile at the coordinates
        /// </summary>
        public Tile GetTile(int column, int row)
        {
            if (!IsValidCoordinate(column, row)) return null;
            return tiles[column][row];
        }

        /// <summary>
        /// Use this method to define the type of tile at the coordinates
        /// </summary>
        public void SetTileType(int column, int row, Tile.Type type)
        {
            if (!IsValidCoordinate(column, row)) return;
            tiles[column][row].type = type;
        }

        /// <summary>
        /// Use this method to check if the coordinates are within the limits
        /// </summary>
        public bool IsValidCoordinate(int column, int row)
        {
            return !(row < 0 || row >= rows || column < 0 || column >= columns);
        }

        /// <summary>
        /// Refresh bitmask to region define by coordinates, width and height
        /// </summary>
        public void RefreshBitmask(int column, int row, int width, int height)
        {
            for (int c = column; c < column + width; c++)
            {
                for (int r = row; r < row + height; r++)
                {
                    if (IsValidCoordinate(c, r))
                    {
                        int mask = GetTileBitmask(c, r);
                        tiles[c][r].bitmask = mask;
                    }
                }
            }
        }

        private int GetTileBitmask(int column, int row)
        {
            int mask = 0;
            int bit = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {

                    int c = column + j;
                    int r = row + i;
                    if (c != column || r != row)
                    {
                        if (!IsValidCoordinate(c, r)) mask |= 1 << bit;
                        else if (tiles[c][r].type == Tile.Type.WALL) mask |= 1 << bit;
                        bit++;
                    }
                }
            }
            return mask;
        }

        public bool[][] GetMap()
        {
            bool[][] map = new bool[columns][];
            for (int column = 0; column < columns; column++)
            {
                map[column] = new bool[rows];
                for (int row = 0; row < rows; row++)
                    map[column][row] = tiles[column][row].type == Tile.Type.WALL;
            }

            return map;
        }

        public Tile GetTileFromPoint(float x, float y)
        {
            int column = (int)Math.Floor(x / Tile.size);
            int row = (int)Math.Floor(y / Tile.size);

            if (!IsValidCoordinate(column, row)) return null;
            return tiles[column][row];
        }

        public Tile GetTileFromPoint(Vector2 point)
        {
            return GetTileFromPoint(point.X, point.Y);
        }

        public Vector2 GetPositionFromTile(Tile tile)
        {
             return new Vector2(tile.column * Tile.size, tile.row * Tile.size);
        }

        public Vector2 GetPositionFromCoordinates((int column, int row) coordinates)
        {
            return new Vector2(coordinates.column * Tile.size, coordinates.row * Tile.size);
        }

        public (int column, int row)? GetCoordinatesFromPoint(float x, float y)
        {
            Tile tile = GetTileFromPoint(x, y);
            if (tile == null) return null;
            return (tile.column, tile.row);
        }

        public (int column, int row)? GetCoordinatesFromPoint(Vector2 point)
        {
            return GetCoordinatesFromPoint(point.X, point.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawFilling(spriteBatch);
            DrawOutlines(spriteBatch);
            DrawShadows(spriteBatch);
        }

        public void DrawFilling(SpriteBatch spriteBatch)
        {
            for (int column = 0; column < columns; column++)
                for (int row = 0; row < rows; row++)
                    tiles[column][row].DrawFilling(spriteBatch,wallsColor, floorColor, position);
        }

        public void DrawOutlines(SpriteBatch spriteBatch)
        {
            for (int column = 0; column < columns; column++)
                for (int row = 0; row < rows; row++)
                    tiles[column][row].DrawOutline(spriteBatch,outlinesColor, position);
        }

        public void DrawShadows(SpriteBatch spriteBatch)
        {
            for (int column = 0; column < columns; column++)
                for (int row = 0; row < rows; row++)
                    tiles[column][row].DrawShadow(spriteBatch,shadowsColor, position);
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            for (int column = 0; column < columns; column++)
                for (int row = 0; row < rows; row++)
                    tiles[column][row].DrawDebug(spriteBatch, Color.Green, position);
        }

    }
}
