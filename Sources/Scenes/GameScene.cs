using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Services;
using Animapix.TileMap;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Robotwarfare
{
    public class GameScene : Scene
    {
        private TileMap _tileMap;
        private Texture2D selectionCursorTex;
        private Vector2? cursorPosition;
        private (int column, int row)? startPosition;
        private (int column, int row)? endPosition;
        private List<(int column, int row)> path;

        public override void Load()
        {
            AssetsService assets = ServiceLocator.GetService<AssetsService>();

            selectionCursorTex = assets.GetAsset<Texture2D>("SelectionCursor");

            _tileMap = new TileMap(60, 30, assets.GetAsset<Texture2D>("TileSet"));
            GenerateNewMap();
            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            if (inputs.IsJustPressed(Keys.Space)) GenerateNewMap();

            // Cursor position
            Tile tile = _tileMap.GetTileFromPoint(inputs.MousePosition());
            if (tile != null)
                cursorPosition = _tileMap.GetPositionFromTile(tile);
            else
                cursorPosition = null;

            // Pathfinding selection
            if (inputs.IsJustPressed(MouseButton.Left, ButtonState.Released) && inputs.MousePosition().X > 0 && inputs.MousePosition().Y > 0)
            {
                Tile selectedTile = _tileMap.GetTileFromPoint(inputs.MousePosition());
                if (selectedTile != null)
                {
                    if (startPosition != null && endPosition == null && selectedTile.type == Tile.Type.FLOOR)
                    {
                        endPosition = _tileMap.GetCoordinatesFromPoint(inputs.MousePosition());
                        path = AStar.FindPath(((int column, int row))startPosition, ((int column, int row))endPosition, _tileMap);
                    }
                    else
                    {
                        if (selectedTile.type == Tile.Type.FLOOR)
                        {
                            startPosition = _tileMap.GetCoordinatesFromPoint(inputs.MousePosition());
                            endPosition = null;
                            path = null;
                        }
                    }
                }
            }
            if (inputs.IsJustPressed(MouseButton.Right, ButtonState.Released))
            {
                endPosition = null;
                startPosition = null;
                path = null;
            }

            base.Update(gameTime);
        }

        public override void Draw()
        {
            _tileMap.Draw(spriteBatch);
            //_tileMap.DrawDebug(spriteBatch);

            if (cursorPosition != null)
                spriteBatch.Draw(selectionCursorTex, (Vector2)cursorPosition, Color.CornflowerBlue);

            if (startPosition != null)
                spriteBatch.Draw(selectionCursorTex, _tileMap.GetPositionFromCoordinates(((int column,int row))startPosition), Color.Red);

            if (endPosition != null)
                spriteBatch.Draw(selectionCursorTex, _tileMap.GetPositionFromCoordinates(((int column, int row))endPosition), Color.Green);

            if(path!= null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Vector2 point1 = new Vector2(path[i].column, path[i].row) * Tile.size + new Vector2(Tile.size / 2, Tile.size / 2);
                    Vector2 point2 = new Vector2(path[i + 1].column, path[i + 1].row) * Tile.size + new Vector2(Tile.size / 2, Tile.size / 2);
                    DrawUtils.Line(point1, point2, Color.White);
                }
            }

            base.Draw();
        }

        private void GenerateNewMap()
        {
            bool[][] map = MapGenerator.Generate(_tileMap.columns, _tileMap.rows);
            _tileMap.Fill(map);
            endPosition = null;
            startPosition = null;
            path = null;
        }

        public override void Unload()
        {
            base.Unload();
        }
    }
}
