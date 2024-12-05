using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCheckers : MonoBehaviour
{
    public GameObject blackTilePreFabs;
    public GameObject whiteTilePreFabs;
    public GameObject blackPiecePreFabs;
    public GameObject whitePiecePreFabs;
    public GameObject blackKingPreFabs;
    public GameObject whiteKingPreFabs;

    private GameController gameController;
    private Board board;
    private MoveLogger moveLogger;
    private List<Move> moves = new List<Move>();

    public GameObject moveLogContent;
    public GameObject moveLogEntryPrefab;

    public GameObject gameOverPanel;
    public TMPro.TextMeshProUGUI gameOverText;
    public Button newGameButton;

    void Start()
    {
        newGameButton.onClick.AddListener(StartNewGame);
        StartNewGame();
    }

    public void StartNewGame()
    {
        PieceColor selectedColor = (PieceColor)PlayerPrefs.GetInt("SelectedColor", (int)PieceColor.WHITE);

        if (board != null)
        {
            moves.Clear();
            moveLogger.ClearMoveLog();
            board.DestroyBoard();
            Destroy(board);
        }

        board = gameObject.AddComponent<Board>();
        board.SetTilesPreFabs(whiteTilePreFabs, blackTilePreFabs);
        board.SetPiecesPreFabs(whitePiecePreFabs, blackPiecePreFabs, whiteKingPreFabs, blackKingPreFabs);
        board.CreateBoard(selectedColor);

        moveLogger = gameObject.AddComponent<MoveLogger>();
        moveLogger.SetMoveLogger(moveLogContent, moveLogEntryPrefab);

        gameController = new GameController(board, moveLogger, moves);

        gameController.OnGameOver += HandleGameOver;
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                gameController.MakeAction(clickedObject);
            }
        }
    }

    private void HandleGameOver(string message)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = message;
        gameController.OnGameOver -= HandleGameOver;
    }
}
