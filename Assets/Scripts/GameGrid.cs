﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
    {
        [System.Serializable]
        public struct PiecePrefab
        {
            public PieceType type;
            public GameObject prefab;
        };

        [System.Serializable]
        public struct PiecePosition
        {
            public PieceType type;
            public int x;
            public int y;
        };
    public Level level;

    public int xDim;
        public int yDim;
        public float fillTime;


        public PiecePrefab[] piecePrefabs;
        public GameObject backgroundPrefab;

        public PiecePosition[] initialPieces;

        private Dictionary<PieceType, GameObject> _piecePrefabDict;

        private GamePiece[,] _pieces;

        private bool _inverse;

        private GamePiece _pressedPiece;
        private GamePiece _enteredPiece;

        private bool _gameOver;

        public bool IsFilling { get; private set; }

    public AudioSource sfx;
    private InGameSFX _sound;

        private void Awake()
        {

        _sound = sfx.GetComponent<InGameSFX>();

        // заповнення словника типами piece prefabs
        _piecePrefabDict = new Dictionary<PieceType, GameObject>();
            for (int i = 0; i < piecePrefabs.Length; i++)
            {
                if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
                {
                    _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
                }
            }

            // backgrounds
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                    background.transform.parent = transform;
                }
            }

            // pieces
            _pieces = new GamePiece[xDim, yDim];

            for (int i = 0; i < initialPieces.Length; i++)
            {
                if (initialPieces[i].x >= 0 && initialPieces[i].y < xDim
                                            && initialPieces[i].y >=0 && initialPieces[i].y <yDim)
                {
                    SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
                }
            }

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (_pieces[x, y] == null)
                    {
                        SpawnNewPiece(x, y, PieceType.Empty);
                    }                
                }
            }

            StartCoroutine(Fill());
        }

        private IEnumerator Fill()
        {        
            bool needsRefill = true;
            IsFilling = true;

            while (needsRefill)
            {
                yield return new WaitForSeconds(fillTime);
                while (FillStep())
                {
                    _inverse = !_inverse;
                    yield return new WaitForSeconds(fillTime);
                }

                needsRefill = ClearAllValidMatches();
            }

            IsFilling = false;
        }

    // проходить через усі комірки сітки, переміщує на 1 вниз (якщо можливо)
    // true якщо хоча б щось перемістилось
    private bool FillStep()
        {
            bool movedPiece = false;
        // y = 0 верх, ігноруємо останній рядок (не можна перемістити вниз)
        for (int y = yDim - 2; y >= 0; y--)
            {
                for (int loopX = 0; loopX < xDim; loopX++)
                {
                    int x = loopX;
                    if (_inverse) { x = xDim - 1 - loopX; }
                    GamePiece piece = _pieces[x, y];

                    if (!piece.IsMovable()) continue;
                
                    GamePiece pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.Empty)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.Empty);
                        movedPiece = true;
                    }
                }
            }

        // найвищий рядок (0) особливий випадок, якщо порожній спавним нові pieces
        for (int x = 0; x < xDim; x++)
            {
                GamePiece pieceBelow = _pieces[x, 0];

                if (pieceBelow.Type != PieceType.Empty) continue;
            
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = Instantiate(_piecePrefabDict[PieceType.Normal], GetWorldPosition(x, -1), Quaternion.identity, this.transform);

                _pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                _pieces[x, 0].Init(x, -1, this, PieceType.Normal);
                _pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                _pieces[x, 0].ColorComponent.SetColor((ColorType)Random.Range(0, _pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }

            return movedPiece;
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(
                transform.position.x - xDim / 2.0f + x,
                transform.position.y + yDim / 2.0f - y);
        }

        private GamePiece SpawnNewPiece(int x, int y, PieceType type)
        {
            GameObject newPiece = Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity, this.transform);
            _pieces[x, y] = newPiece.GetComponent<GamePiece>();
            _pieces[x, y].Init(x, y, this, type);

            return _pieces[x, y];
        }

        private static bool IsAdjacent(GamePiece piece1, GamePiece piece2) =>
            (piece1.X == piece2.X && Mathf.Abs(piece1.Y - piece2.Y) == 1) ||
            (piece1.Y == piece2.Y && Mathf.Abs(piece1.X - piece2.X) == 1);

        private void SwapPieces(GamePiece piece1, GamePiece piece2)
        {
            if (_gameOver) { return; }

            if (!piece1.IsMovable() || !piece2.IsMovable()) return;
        
            _pieces[piece1.X, piece1.Y] = piece2;
            _pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || 
                GetMatch(piece2, piece1.X, piece1.Y) != null ||
                piece1.Type == PieceType.Rainbow ||
                piece2.Type == PieceType.Rainbow ||
                piece1.Type == PieceType.Bomb ||
                piece2.Type == PieceType.Bomb)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);

                 if ((piece1.Type == PieceType.Rainbow || piece1.Type == PieceType.Bomb)
                && piece1.IsClearable() && piece2.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                }

                if ((piece2.Type == PieceType.Rainbow || piece2.Type == PieceType.Bomb)
                && piece2.IsClearable() && piece1.IsColored())
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                    if (clearColor)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                }

                ClearAllValidMatches();

                if (piece1.Type == PieceType.RowClear || piece1.Type == PieceType.ColumnClear)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }

                if (piece2.Type == PieceType.RowClear || piece2.Type == PieceType.ColumnClear)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }

                _pressedPiece = null;
                _enteredPiece = null;

                StartCoroutine(Fill());

            level.OnMove();
        }
            else
            {
                _pieces[piece1.X, piece1.Y] = piece1;
                _pieces[piece2.X, piece2.Y] = piece2;
            }
        }

        public void PressPiece(GamePiece piece) => _pressedPiece = piece;

        public void EnterPiece(GamePiece piece) => _enteredPiece = piece;

        public void ReleasePiece()
        {
            if (IsAdjacent (_pressedPiece, _enteredPiece))
            {
                SwapPieces(_pressedPiece, _enteredPiece);
            }
        }

        private bool ClearAllValidMatches()
        {
            bool needsRefill = false;

            for (int y = 0; y < yDim; y++)
            {
                for (int x = 0; x < xDim; x++)
                {
                    if (!_pieces[x, y].IsClearable()) continue;
                
                    List<GamePiece> match = GetMatch(_pieces[x, y], x, y);

                    if (match == null) continue;
                
                    PieceType specialPieceType = PieceType.Count;
                    GamePiece randomPiece = match[Random.Range(0, match.Count)];
                    int specialPieceX = randomPiece.X;
                    int specialPieceY = randomPiece.Y;

                    // Spawning special pieces
                    if (match.Count == 4)
                    {
                        if (_pressedPiece == null || _enteredPiece == null)
                        {
                            specialPieceType = (PieceType) Random.Range((int) PieceType.RowClear, (int) PieceType.ColumnClear);
                        }
                        else if (_pressedPiece.Y == _enteredPiece.Y)
                        {
                            specialPieceType = PieceType.RowClear;
                        }
                        else
                        {
                            specialPieceType = PieceType.ColumnClear;
                        }
                    } // Spawning a rainbow piece or bomb
                    else if (match.Count == 5)
                    {
                    specialPieceType = PieceType.Rainbow;
                }
                else if (match.Count > 5)
                {
                    specialPieceType = PieceType.Bomb;
                }

                    foreach (var gamePiece in match)
                    {
                        if (!ClearPiece(gamePiece.X, gamePiece.Y)) continue;
                    
                        needsRefill = true;

                        if (gamePiece != _pressedPiece && gamePiece != _enteredPiece) continue;
                    
                        specialPieceX = gamePiece.X;
                        specialPieceY = gamePiece.Y;
                    }

                    // Setting their colors
                    if (specialPieceType == PieceType.Count) continue;
                
                    Destroy(_pieces[specialPieceX, specialPieceY]);
                    GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                    if ((specialPieceType == PieceType.RowClear || specialPieceType == PieceType.ColumnClear) 
                        && newPiece.IsColored() && match[0].IsColored())
                    {
                        newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                    }
                    else if (specialPieceType == PieceType.Rainbow && newPiece.IsColored())
                    {
                        newPiece.ColorComponent.SetColor(ColorType.Any);
                    }
                }
            }

            return needsRefill;
        }

        private List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
        {
            if (!piece.IsColored()) return null;
            var color = piece.ColorComponent.Color;
            var horizontalPieces = new List<GamePiece>();
            var verticalPieces = new List<GamePiece>();
            var matchingPieces = new List<GamePiece>();

            // First check horizontal
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if (dir == 0)
                    { // Left
                        x = newX - xOffset;
                    }
                    else
                    { // right
                        x = newX + xOffset;                        
                    }

                    // out-of-bounds
                    if (x < 0 || x >= xDim) { break; }

                    // piece is the same color?
                    if (_pieces[x, newY].IsColored() && _pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(_pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                matchingPieces.AddRange(horizontalPieces);
            }

            // Traverse vertically if we found a match (for L and T shape)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++ )
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)                        
                        {
                            int y;
                            
                            if (dir == 0)
                            { // Up
                                y = newY - yOffset;
                            }
                            else
                            { // Down
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= yDim)
                            {
                                break;
                            }

                            if (_pieces[horizontalPieces[i].X, y].IsColored() && _pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        matchingPieces.AddRange(verticalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }


            // Didn't find anything going horizontally first,
            // so now check vertically
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < xDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    { // Up
                        y = newY - yOffset;
                    }
                    else
                    { // Down
                        y = newY + yOffset;                        
                    }

                    // out-of-bounds
                    if (y < 0 || y >= yDim) { break; }

                    // piece is the same color?
                    if (_pieces[newX, y].IsColored() && _pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(_pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                matchingPieces.AddRange(verticalPieces);
            }

            // Traverse horizontally if we found a match (for L and T shape)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                            { // Left
                                x = newX - xOffset;
                            }
                            else
                            { // Right
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }

                            if (_pieces[x, verticalPieces[i].Y].IsColored() && _pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        matchingPieces.AddRange(horizontalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            return null;
        }

    private bool ClearPiece(int x, int y)
    {
        if (!_pieces[x, y].IsClearable() || _pieces[x, y].ClearableComponent.IsBeingCleared) return false;

        _sound.PlaySound(_sound.pieceCleared);
        _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.Empty);

            return true;

        }

        

        public void ClearRow(int row)
        {
            for (int x = 0; x < xDim; x++)
            {
                ClearPiece(x, row);
            }
        }

        public void ClearColumn(int column)
        {
            for (int y = 0; y < yDim; y++)
            {
                ClearPiece(column, y);
            }
        }

        public void ClearColor(ColorType color)
        {
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if ((_pieces[x, y].IsColored() && _pieces[x, y].ColorComponent.Color == color)
                        || (color == ColorType.Any))
                    {
                        ClearPiece(x, y);
                    }
                }
            }
        }


    public void ClearBomb(int x, int y)
    {
        // Пройдемося по клітинках у радіусі 3 від заданої позиції (x, y)
        for (int offsetX = -3; offsetX <= 3; offsetX++)
        {
            for (int offsetY = -3; offsetY <= 3; offsetY++)
            {
                // Перевіряємо, чи позиція (x + offsetX, y + offsetY) знаходиться в межах ігрового поля
                if (IsWithinBounds(x + offsetX, y + offsetY))
                {
                    // Перевіряємо, чи елемент знаходиться у межах круга радіусом 3 клітинки від заданої позиції (x, y)
                    if (Mathf.Abs(offsetX) + Mathf.Abs(offsetY) <= 3)
                    {
                        // Видаляємо елемент
                        ClearPiece(x + offsetX, y + offsetY);
                    }
                }
            }
        }
    }

    // Метод для перевірки, чи задані координати знаходяться в межах ігрового поля
    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < xDim && y >= 0 && y < yDim;
    }


    public void GameOver()
    {
        _sound.PlaySound(_sound.gameEnd);
        _gameOver = true;
    }

        public List<GamePiece> GetPiecesOfType(PieceType type)
        {
            var piecesOfType = new List<GamePiece>();

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    if (_pieces[x, y].Type == type)
                    {
                        piecesOfType.Add(_pieces[x, y]);
                    }
                }
            }

            return piecesOfType;
        }

    }

