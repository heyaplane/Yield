using System;
using System.Collections.Generic;
using UnityEngine;

public struct ChunkCoordinate : IComparable<ChunkCoordinate>
{
    public int chunkRow;
    public int chunkCol;
    public int imageRow;
    public int imageCol;
    public Vector2 startingWorldSpacePos;

    public ChunkCoordinate(int chunkRow, int chunkCol, int imageRow, int imageCol, Vector2 startingWorldSpacePos)
    {
        this.chunkRow = chunkRow;
        this.chunkCol = chunkCol;
        this.imageRow = imageRow;
        this.imageCol = imageCol;
        this.startingWorldSpacePos = startingWorldSpacePos;
    }

    public ChunkCoordinate(ChunkCoordinate coordinateToCopy)
    {
        chunkRow = coordinateToCopy.chunkRow;
        chunkCol = coordinateToCopy.chunkCol;
        imageRow = coordinateToCopy.imageRow;
        imageCol = coordinateToCopy.imageCol;
        startingWorldSpacePos = coordinateToCopy.startingWorldSpacePos;
    }

    public (int, int)[] GetNeighboringChunks(MapSO currentMap)
    {
        var neighborChunks = new List<(int, int)>();
        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            int neighborChunkRow = chunkRow + rowOffset;
            if (neighborChunkRow < 0 || neighborChunkRow == currentMap.NumRows) continue;

            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                int neighborChunkCol = chunkCol + colOffset;
                if (neighborChunkCol < 0 || neighborChunkCol == currentMap.NumCols) continue;
                
                neighborChunks.Add((neighborChunkRow, neighborChunkCol));
            }
        }

        return neighborChunks.ToArray();
    }

    public ChunkCoordinate[] GetNeighboringCoordinates(MapSO currentMap, ChunkSO currentChunk, Vector2 worldSpacePos)
    {
        var neighbors = new List<ChunkCoordinate>();

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            int neighborChunkRow = chunkRow;
            int neighborRow = imageRow + rowOffset;
            if (neighborRow < 0)
            {
                neighborRow = currentChunk.ChunkSize - 1;
                neighborChunkRow--;
                if (neighborChunkRow < 0) continue;
            }
            
            else if (neighborRow == currentChunk.ChunkSize)
            {
                neighborRow = 0;
                neighborChunkRow++;
                if (neighborChunkRow == currentMap.NumRows) continue;
            }
            
            for (int colOffset = -1; colOffset <= 1; colOffset++)
            {
                int neighborChunkCol = chunkCol;
                int neighborCol = imageCol + colOffset;
                
                if (neighborCol < 0)
                {
                    neighborCol = currentChunk.ChunkSize - 1;
                    neighborChunkCol--;
                    if (neighborChunkCol < 0) continue;
                }
                
                else if (neighborCol == currentChunk.ChunkSize)
                {
                    neighborCol = 0;
                    neighborChunkCol++;
                    if (neighborChunkCol == currentMap.NumCols) continue;
                }

                var startingPos = new Vector2(
                    worldSpacePos.x + colOffset * currentChunk.CoordinateWorldSize, 
                    worldSpacePos.y - rowOffset * currentChunk.CoordinateWorldSize);
                
                neighbors.Add(new ChunkCoordinate(
                    neighborChunkRow, neighborChunkCol, 
                    neighborRow, neighborCol, startingPos));
            }
        }

        return neighbors.ToArray();
    }
    
    public int CompareTo(ChunkCoordinate other)
    {
        int chunkRowComparison = chunkRow.CompareTo(other.chunkRow);
        if (chunkRowComparison != 0) return chunkRowComparison;
        int chunkColComparison = chunkCol.CompareTo(other.chunkCol);
        if (chunkColComparison != 0) return chunkColComparison;
        int imageRowComparison = imageRow.CompareTo(other.imageRow);
        if (imageRowComparison != 0) return imageRowComparison;
        return imageCol.CompareTo(other.imageCol);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var item = (ChunkCoordinate) obj;
        return chunkRow.Equals(item.chunkRow) && chunkCol.Equals(item.chunkCol) && imageRow.Equals(item.imageRow) && imageCol.Equals(item.imageCol);
    }

    public override int GetHashCode() => HashCode.Combine(chunkRow, chunkCol, imageRow, imageCol);
}