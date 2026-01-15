# Quick Reference Guide

## For Developers

### File Structure
```
ChessGame/
├── Client/                      # Client application
│   ├── ChessBoard.cs           # Chess logic and validation
│   ├── ChessGameForm.cs        # Game UI (button-based board)
│   ├── ClientForm.cs           # Connection UI
│   ├── TcpClient.cs            # TCP server connection
│   └── Client.csproj           # Client project file
├── ChessGame/                   # Server application
│   ├── TcpServer.cs            # TCP server with message relay
│   ├── ServerForm.cs           # Server UI
│   └── Server.csproj           # Server project file
└── Documentation/
    ├── IMPLEMENTATION_NOTES.md # Detailed implementation guide
    ├── ARCHITECTURE.md          # System architecture and diagrams
    └── TESTING_CHECKLIST.md     # 22-point test suite
```

### Key Classes

#### ChessBoard
```csharp
// Create board
ChessBoard board = new ChessBoard();

// Check valid move
bool valid = board.IsValidMoveSafe(fromRow, fromCol, toRow, toCol);

// Move piece
board.MovePiece(fromRow, fromCol, toRow, toCol);

// Check game state
bool inCheck = board.IsInCheck(PieceColor.White);
bool checkmate = board.IsCheckmate(PieceColor.White);
```

#### ChessGameForm
```csharp
// Create game form
ChessGameForm game = new ChessGameForm(
    playerColor,
    playerName,
    opponentName
);

// Handle game events
game.OnGameMessage += (msg) => {
    // Send to server via TCP
};
game.OnGameExited += () => {
    // Cleanup
};
```

### Message Protocols

#### TCP Messages
| Format | Example | Purpose |
|--------|---------|---------|
| `PlayerName` | `Alice` | Initial handshake |
| `[OPPONENT]\|name\|color` | `[OPPONENT]\|Bob\|WHITE` | Match info |
| `[UDP_PORT]port` | `[UDP_PORT]12345` | Share UDP port |
| `[UDP_INFO]ip\|port` | `[UDP_INFO]192.168.1.5\|54321` | Opponent endpoint |
| `[PAUSE]name` | `[PAUSE]Alice` | Pause request |
| `[EXIT]name` | `[EXIT]Bob` | Player quit |

#### UDP Messages
| Format | Example | Purpose |
|--------|---------|---------|
| `r1,c1->r2,c2` | `6,4->4,4` | Chess move (e2→e4) |
| `[CHECKMATE]` | `[CHECKMATE]` | Game over |

### Chess Piece Movement Rules

| Piece | Movement | Notes |
|-------|----------|-------|
| ♙ Pawn | Forward 1 (2 from start), diagonal capture | No en passant |
| ♖ Rook | Horizontal/vertical unlimited | Must be clear path |
| ♘ Knight | L-shape (2+1 or 1+2) | Can jump over pieces |
| ♗ Bishop | Diagonal unlimited | Must be clear path |
| ♕ Queen | Rook + Bishop combined | Most powerful |
| ♔ King | 1 square any direction | No castling |

### Board Coordinates
```
    0   1   2   3   4   5   6   7
  +---+---+---+---+---+---+---+---+
0 | ♜ | ♞ | ♝ | ♛ | ♚ | ♝ | ♞ | ♜ |  Black
  +---+---+---+---+---+---+---+---+
1 | ♟ | ♟ | ♟ | ♟ | ♟ | ♟ | ♟ | ♟ |
  +---+---+---+---+---+---+---+---+
2 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
3 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
4 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
5 |   |   |   |   |   |   |   |   |
  +---+---+---+---+---+---+---+---+
6 | ♙ | ♙ | ♙ | ♙ | ♙ | ♙ | ♙ | ♙ |
  +---+---+---+---+---+---+---+---+
7 | ♖ | ♘ | ♗ | ♕ | ♔ | ♗ | ♘ | ♖ |  White
  +---+---+---+---+---+---+---+---+
```

### Color Scheme
- **Board squares**: Beige (#F5F5DC) and Brown (#A52A2A)
- **Selected piece**: Gold (#FFD700)
- **Valid empty move**: LightGreen (#90EE90)
- **Capturable enemy**: IndianRed (#CD5C5C)
- **White pieces**: White symbols (♔♕♖♗♘♙)
- **Black pieces**: Black symbols (♚♛♜♝♞♟)

### Common Tasks

#### Add New Chess Rule
1. Edit `ChessBoard.cs`
2. Update `BasicMove()` or add new validation method
3. Call from `IsValidMove()`
4. Test thoroughly

#### Change Board Colors
1. Edit `ChessGameForm.cs`
2. Find `InitializeChessBoard()` method
3. Change `Color.Beige` and `Color.Brown` to desired colors

#### Add New Message Type
1. Define format in both server and client
2. Server: Add handler in `TcpServer.HandleClientAsync()`
3. Client: Add handler in `ClientForm.TcpConnection_OnMessageReceived()`
4. Update documentation

#### Debug UDP Issues
1. Check firewall allows UDP on dynamic ports
2. Verify both clients receive `[UDP_INFO]` message
3. Check `GetLocalPort()` returns valid port
4. Enable logging in `UdpGameClient.OnGameMessage` event

### Build & Deploy

#### Debug Build
```
1. Open Visual Studio
2. Set "Debug" configuration
3. F5 to build and run
4. Debugger attached
```

#### Release Build
```
1. Set "Release" configuration
2. Build → Build Solution (Ctrl+Shift+B)
3. Binaries in bin/Release/
4. Distribute .exe files + .NET Framework 4.8 requirement
```

### Troubleshooting

| Problem | Solution |
|---------|----------|
| Build fails on Linux | Use Windows (requires .NET Framework 4.8) |
| Moves don't appear | Check server relay and message handlers |
| Invalid moves accepted | Verify `IsValidMoveSafe()` is called |
| Board colors wrong | Check Beige/Brown not White/Crimson |
| Pieces display as ? | Font doesn't support Unicode chess symbols |
| Checkmate not detected | Verify `IsCheckmate()` logic |
| Server won't start | Port 5000 might be in use |

### Performance Tips

1. **TCP relay**: All messages through server (reliable but slight latency)
2. **Move validation**: Always validate both locally and on receive
3. **Board updates**: Use `UpdateBoardDisplay()` sparingly
4. **Event handlers**: Keep handlers lightweight, use Invoke for UI

### Security Considerations

⚠️ **Current limitations:**
- No encryption on TCP messages
- Moves validated but no authentication
- No protection against replay attacks
- Server fully trusts all clients

🔒 **For production:**
- Add TLS/SSL for TCP connections
- Add player authentication
- Implement anti-cheat validation
- Rate limiting on moves
- Message signing

### Extension Ideas

- [ ] Add move history panel
- [ ] Implement castling
- [ ] Add en passant
- [ ] Pawn promotion dialog
- [ ] Game timer/clock
- [ ] Draw offers
- [ ] Stalemate detection
- [ ] Save/load game (PGN format)
- [ ] Spectator mode
- [ ] Chat functionality
- [ ] AI opponent (Stockfish integration)

### Contact & Support

**Repository**: https://github.com/lgp1012/ChessGame
**Documentation**: See IMPLEMENTATION_NOTES.md, ARCHITECTURE.md
**Testing**: See TESTING_CHECKLIST.md

## Quick Start

1. **Clone**: `git clone https://github.com/lgp1012/ChessGame`
2. **Open**: `ChessGame.slnx` in Visual Studio
3. **Build**: Both projects (Client + ChessGame)
4. **Run Server**: Start ChessGame.exe → Click "Start Server"
5. **Run Clients**: Start 2x Client.exe → Enter names → Connect to 127.0.0.1
6. **Play**: Click pieces and valid moves to play chess!

Enjoy! ♔♕♖♗♘♙
