using System;
using System.Collections.Generic;

namespace Animapix.TileMap
{
    public static class MapGenerator
    {
        private static Random _random = new Random();

        public static bool[][] Generate(int columns, int rows, float fillFactor = .55f, int simulationSteps = 10, int deathLimit = 4, int birthLimit = 5, int cavesSizeLimit = 0, int IslandsSizeLimit = 0, string seed = null)
        {
            if (seed != null) { _random = new Random(seed.GetHashCode()); }

            bool[][] map = new bool[columns][];
            for (int i = 0; i < columns; i++)
            {
                map[i] = new bool[rows];
                for (int j = 0; j < rows; j++)
                    map[i][j] = new bool();
            }

            RandomFillMap(map, fillFactor);

            for (int i = 0; i < simulationSteps; i++)
                map = CellularAutomataProcesses(map, deathLimit, birthLimit);


            if (IslandsSizeLimit > 0) RemoveRegions(map, true, IslandsSizeLimit);
            if (cavesSizeLimit > 0) RemoveRegions(map, false, cavesSizeLimit);

            List<Room> rooms = new List<Room>();

            foreach (List<CellCoordinates> region in GetRegions(map, false))
                rooms.Add(new Room(region, map));
            rooms.Sort();
            rooms[0].isMainRoom = true;
            rooms[0].isAccessibleFromMainRoom = true;

            ConnectClosestRooms(map, rooms);

            return map;
        }

        private static void RandomFillMap(bool[][] map, float fillFactor)
        {
            int columns = map.Length;
            int rows = map[0].Length;

            for (int column = 0; column < columns; column++)
                for (int row = 0; row < rows; row++)
                {
                    if (column == 0 || row == 0 || column == columns - 1 || row == rows - 1)
                        map[column][row] = true;
                    else
                        map[column][row] = _random.NextDouble() < fillFactor;
                }
        }

        private static bool[][] CellularAutomataProcesses(bool[][] oldCellMap, int deathLimit, int birthLimit)
        {
            int columns = oldCellMap.Length;
            int rows = oldCellMap[0].Length;

            bool[][] newCellMap = new bool[columns][];
            for (int i = 0; i < columns; i++)
            {
                newCellMap[i] = new bool[rows];
                for (int j = 0; j < rows; j++)
                    newCellMap[i][j] = new bool();
            }

            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    int nbs = CountAliveNeighbours(oldCellMap, column, row);

                    if (oldCellMap[column][row])
                    {
                        if (nbs < deathLimit) newCellMap[column][row] = false;
                        else newCellMap[column][row] = true;
                    }
                    else
                    {
                        if (nbs > birthLimit) newCellMap[column][row] = true;
                        else newCellMap[column][row] = false;
                    }
                }
            }

            return newCellMap;
        }

        private static int CountAliveNeighbours(bool[][] cellMap, int column, int row)
        {
            int columns = cellMap.Length;
            int rows = cellMap[0].Length;
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int c = column + i;
                    int r = row + j;

                    if (i != 0 || j != 0)
                    {
                        if (c < 0 || r < 0 || c >= columns || r >= rows)
                            count++;
                        else if (cellMap[c][r])
                            count++;
                    }
                }
            }
            return count;
        }

        private static void RemoveRegions(bool[][] map, bool cellType, int threshold)
        {
            List<List<CellCoordinates>> regions = GetRegions(map, cellType);

            foreach (List<CellCoordinates> region in regions)
            {
                if (region.Count < threshold)
                {
                    foreach (CellCoordinates cell in region)
                    {
                        map[cell.column][cell.row] = !cellType;
                    }
                }
            }
        }

        private static List<List<CellCoordinates>> GetRegions(bool[][] cellMap, bool cellType)
        {
            int columns = cellMap.Length;
            int rows = cellMap[0].Length;
            List<List<CellCoordinates>> regions = new List<List<CellCoordinates>>();

            bool[][] flags = new bool[columns][];
            for (int i = 0; i < columns; i++)
            {
                flags[i] = new bool[rows];
                for (int j = 0; j < rows; j++)
                    flags[i][j] = new bool();
            }

            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (flags[column][row] == false && cellMap[column][row] == cellType)
                    {
                        List<CellCoordinates> newRegion = GetRegion(cellMap, column, row);
                        regions.Add(newRegion);
                        foreach (CellCoordinates cell in newRegion)
                            flags[cell.column][cell.row] = true;
                    }
                }
            }
            return regions;
        }

        private static List<CellCoordinates> GetRegion(bool[][] cellMap, int startColumn, int stratRow)
        {
            int columns = cellMap.Length;
            int rows = cellMap[0].Length;

            List<CellCoordinates> cells = new List<CellCoordinates>();

            bool[][] flags = new bool[columns][];
            for (int i = 0; i < columns; i++)
            {
                flags[i] = new bool[rows];
                for (int j = 0; j < rows; j++)
                    flags[i][j] = new bool();
            }

            bool cellType = cellMap[startColumn][stratRow];

            Queue<CellCoordinates> queue = new Queue<CellCoordinates>();
            queue.Enqueue(new CellCoordinates(startColumn, stratRow));
            flags[startColumn][stratRow] = true;

            while (queue.Count > 0)
            {
                CellCoordinates cell = queue.Dequeue();
                cells.Add(cell);

                for (int c = cell.column - 1; c <= cell.column + 1; c++)
                {
                    for (int r = cell.row - 1; r <= cell.row + 1; r++)
                    {
                        if (IsValidCoordinate(cellMap, c, r) && (c == cell.column || r == cell.row))
                        {
                            if (flags[c][r] == false && cellMap[c][r] == cellType)
                            {
                                flags[c][r] = true;
                                queue.Enqueue(new CellCoordinates(c, r));
                            }
                        }
                    }

                }
            }

            return cells;
        }

        private static bool IsValidCoordinate(bool[][] map, int column, int row)
        {
            int columns = map.Length;
            int rows = map[0].Length;
            return column >= 0 && row >= 0 && column < columns && row < rows;
        }

        private static void ConnectClosestRooms(bool[][] map, List<Room> rooms, bool forceAccesybilityFromMainRoom = false)
        {
            List<Room> roomListA = new List<Room>();
            List<Room> roomListB = new List<Room>();

            if (forceAccesybilityFromMainRoom)
            {
                foreach (Room room in rooms)
                {
                    if (room.isAccessibleFromMainRoom)
                        roomListB.Add(room);
                    else
                        roomListA.Add(room);
                }
            }
            else
            {
                roomListA = rooms;
                roomListB = rooms;
            }

            int bestDistance = 0;
            CellCoordinates bestCellA = new CellCoordinates();
            CellCoordinates bestCellB = new CellCoordinates();
            Room bestRoomA = new Room();
            Room bestRoomB = new Room();
            bool possibleConnectionFound = false;

            foreach (Room roomA in roomListA)
            {
                if (!forceAccesybilityFromMainRoom)
                {
                    possibleConnectionFound = false;
                    if (roomA.connectedRooms.Count > 0)
                        continue;
                }

                foreach (Room roomB in roomListB)
                {
                    if (roomA == roomB || roomA.IsConnected(roomB)) continue;

                    for (int cellIndexA = 0; cellIndexA < roomA.edgeCells.Count; cellIndexA++)
                    {
                        for (int cellIndexB = 0; cellIndexB < roomB.edgeCells.Count; cellIndexB++)
                        {
                            CellCoordinates cellA = roomA.edgeCells[cellIndexA];
                            CellCoordinates cellB = roomB.edgeCells[cellIndexB];
                            int distanceBetweenRooms = (int)(MathF.Pow(cellA.column - cellB.column, 2) + MathF.Pow(cellA.row - cellB.row, 2));
                            if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                            {
                                bestDistance = distanceBetweenRooms;
                                possibleConnectionFound = true;
                                bestCellA = cellA;
                                bestCellB = cellB;
                                bestRoomA = roomA;
                                bestRoomB = roomB;
                            }
                        }
                    }
                }
                if (possibleConnectionFound && !forceAccesybilityFromMainRoom) CreatePassage(map, bestRoomA, bestRoomB, bestCellA, bestCellB);
            }
            if (possibleConnectionFound && forceAccesybilityFromMainRoom)
            {
                CreatePassage(map, bestRoomA, bestRoomB, bestCellA, bestCellB);
                ConnectClosestRooms(map, rooms, true);
            }

            if (!forceAccesybilityFromMainRoom) ConnectClosestRooms(map, rooms, true);
        }

        private static void CreatePassage(bool[][] map, Room roomA, Room roomB, CellCoordinates cellA, CellCoordinates cellB)
        {
            Room.ConnectRoom(roomA, roomB);
            List<CellCoordinates> line = GetLine(cellA, cellB);
            foreach (CellCoordinates cell in line)
                DrawCircle(map, cell, 2);

        }

        private static List<CellCoordinates> GetLine(CellCoordinates from, CellCoordinates to)
        {
            List<CellCoordinates> line = new List<CellCoordinates>();

            int column = from.column;
            int row = from.row;

            int dx = to.column - from.column;
            int dy = to.row - from.row;

            bool inverted = false;
            int step = Math.Sign(dx);
            int gradientStep = Math.Sign(dy);

            int longest = Math.Abs(dx);
            int shortest = Math.Abs(dy);

            if (longest < shortest)
            {
                inverted = true;
                longest = Math.Abs(dy);
                shortest = Math.Abs(dx);

                step = Math.Sign(dy);
                gradientStep = Math.Sign(dx);
            }

            int gradientAccumulation = longest / 2;

            for (int i = 0; i < longest; i++)
            {
                line.Add(new CellCoordinates(column, row));
                if (inverted) row += step;
                else column += step;

                gradientAccumulation += shortest;
                if (gradientAccumulation >= longest)
                {
                    if (inverted) column += gradientStep;
                    else row += gradientStep;
                    gradientAccumulation -= longest;
                }
            }

            return line;
        }

        private static void DrawCircle(bool[][] map, CellCoordinates cell, int radius)
        {
            for (int column = -radius; column < radius; column++)
            {
                for (int row = -radius; row < radius; row++)
                {
                    if (column * column + row * row < radius * radius)
                    {
                        int drawColumn = cell.column + column;
                        int drawRow = cell.row + row;
                        if (IsValidCoordinate(map, drawColumn, drawRow))
                            map[drawColumn][drawRow] = false;
                    }
                }
            }
        }

        private struct CellCoordinates
        {
            public int column;
            public int row;

            public CellCoordinates(int column, int row)
            {
                this.column = column;
                this.row = row;
            }
        }

        private class Room : IComparable<Room>
        {
            public List<CellCoordinates> cells;
            public List<CellCoordinates> edgeCells;
            public List<Room> connectedRooms;
            public int roomSize;
            public bool isAccessibleFromMainRoom;
            public bool isMainRoom;

            public Room() { }

            public Room(List<CellCoordinates> cells, bool[][] map)
            {
                this.cells = cells;
                roomSize = cells.Count;
                connectedRooms = new List<Room>();
                edgeCells = new List<CellCoordinates>();

                foreach (CellCoordinates cell in cells)
                    for (int c = cell.column - 1; c <= cell.column + 1; c++)
                        for (int r = cell.row - 1; r < cell.row + 1; r++)
                            if (c == cell.column || r == cell.row)
                                edgeCells.Add(cell);
            }

            public static void ConnectRoom(Room roomA, Room roomB)
            {
                if (roomA.isAccessibleFromMainRoom) roomB.SetAccessibleFromMainRoom();
                if (roomB.isAccessibleFromMainRoom) roomA.SetAccessibleFromMainRoom();
                roomA.connectedRooms.Add(roomB);
                roomB.connectedRooms.Add(roomA);
            }

            public int CompareTo(Room other)
            {
                return other.roomSize.CompareTo(roomSize);
            }

            public bool IsConnected(Room otherRoom)
            {
                return connectedRooms.Contains(otherRoom);
            }

            public void SetAccessibleFromMainRoom()
            {
                if (!isAccessibleFromMainRoom)
                {
                    isAccessibleFromMainRoom = true;
                    foreach (Room room in connectedRooms)
                        room.SetAccessibleFromMainRoom();
                }
            }
        }
    }
}
