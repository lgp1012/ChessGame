# Chess Game with TCP Communication

A complete multiplayer chess game built with C# .NET Framework 4.8, featuring real-time gameplay over TCP with server relay for all game communication.

## 🎯 Features

### Complete Chess Logic
- ✅ Full move validation for all chess pieces
- ✅ Check and checkmate detection
- ✅ Legal move enforcement (prevents self-check)
- ✅ Path blocking detection (bishops, rooks, queens)
- ✅ Special pawn rules (double move from start, diagonal capture)

### Interactive UI
- 🎨 Button-based chessboard with **Beige & Brown** squares
- 🎭 Unicode chess symbols: ♔♕♖♗♘♙ (White) & ♚♛♜♝♞♟ (Black)
- 🎯 Move highlighting:
  - **Gold**: Selected piece
  - **Light Green**: Valid empty destination
  - **Indian Red**: Capturable enemy piece
- 🖱️ Click-to-move interface with visual feedback

### Network Architecture
- 🌐 **TCP Server**: Matchmaking, coordination, and message relay
- 🔄 **TCP Protocol**: All communication through server relay
- 🔒 **Move Validation**: Both client and opponent moves validated

## 🚀 Quick Start

### Prerequisites
- **OS**: Windows 10 or 11
- **Framework**: .NET Framework 4.8
- **IDE**: Visual Studio 2019 or later

### Build & Run

1. **Clone the repository**
   ```bash
   git clone https://github.com/lgp1012/ChessGame.git
   cd ChessGame
   ```

2. **Open in Visual Studio**
   - Open `ChessGame.slnx`
   - Restore any NuGet packages if prompted

3. **Build both projects**
   - Right-click Solution → Build Solution (Ctrl+Shift+B)
   - Ensure both `Client` and `ChessGame` build successfully

4. **Start the server**
   - Run `ChessGame` project
   - Click **"Start Server"** button
   - Server listens on **port 5000**

5. **Connect clients**
   - Run `Client` project (start 2 instances)
   - Enter player name
   - Enter server IP: `127.0.0.1` (for localhost)
   - Click **"Connect"**

6. **Play chess!**
   - Server auto-matches when 2 clients connect
   - White player goes first
   - Click pieces and valid squares to move

## 📖 Documentation

| Document | Description |
|----------|-------------|
| [IMPLEMENTATION_NOTES.md](IMPLEMENTATION_NOTES.md) | Detailed implementation guide and technical specs |
| [ARCHITECTURE.md](ARCHITECTURE.md) | System architecture, diagrams, and data flow |
| [TESTING_CHECKLIST.md](TESTING_CHECKLIST.md) | Comprehensive 22-point test suite |
| [QUICK_REFERENCE.md](QUICK_REFERENCE.md) | Developer quick reference and code examples |

## 🎮 How to Play

### Starting a Game
1. **White player** sees: "Your Turn" (green)
2. **Black player** sees: "Bob's Turn" (blue)

### Making a Move
1. Click your piece (highlights in **Gold**)
2. Valid moves show in **Light Green** or **Indian Red** (enemy)
3. Click destination to move
4. Move transmits via UDP to opponent
5. Turn switches automatically

### Game Controls
- **Pause**: Temporarily pause the game
- **Exit**: Quit and end the match

### Winning
- Checkmate opponent's king
- Game automatically detects and announces winner

## 🏗️ Architecture

```
┌─────────────┐           TCP            ┌─────────────┐
│  Client A   │ ◄──────────────────────► │ TCP Server  │
│   (White)   │      Port 5000           │  (Relay)    │
└─────────────┘                          └─────────────┘
                                                 │
                                                 │ TCP
                                                 ▼
                                          ┌─────────────┐
                                          │  Client B   │
                                          │   (Black)   │
                                          └─────────────┘
```

### Communication Flow
1. **TCP**: Initial connection, matchmaking, opponent info
2. **TCP Relay**: Server relays all game messages between clients
3. **TCP Gameplay**: All moves transmitted through server
4. **TCP Control**: Pause, exit, server commands

## 🧩 Project Structure

```
ChessGame/
├── Client/                         # Client Application
│   ├── ChessBoard.cs              # Chess logic & validation
│   ├── ChessGameForm.cs           # Game UI (button board)
│   ├── ClientForm.cs              # Connection UI
│   ├── TcpClient.cs               # TCP server connection
│   └── Client.csproj              # Client project
├── ChessGame/                      # Server Application
│   ├── TcpServer.cs               # TCP server with message relay
│   ├── ServerForm.cs              # Server UI
│   └── Server.csproj              # Server project
└── Documentation/
    ├── IMPLEMENTATION_NOTES.md
    ├── ARCHITECTURE.md
    ├── TESTING_CHECKLIST.md
    └── QUICK_REFERENCE.md
```

## 🔧 Key Components

### ChessBoard.cs
Complete chess engine with:
- Move validation for all piece types
- Check/checkmate detection
- Path blocking analysis
- Safe move verification (prevents self-check)

### ChessGameForm.cs
Interactive game UI featuring:
- 8×8 button grid
- Unicode piece symbols
- Move highlighting system
- Real-time board updates

### TcpServer.cs
Server coordination and relay:
- Client matchmaking (pairs of 2)
- Color assignment (White/Black)
- Message relay between clients
- Game lifecycle management

## 📋 Chess Rules Implemented

| Piece | Movement | Special Rules |
|-------|----------|---------------|
| ♙ Pawn | Forward 1 (2 from start) | Diagonal capture only |
| ♖ Rook | Horizontal/Vertical unlimited | Clear path required |
| ♘ Knight | L-shape (2+1 or 1+2) | Can jump over pieces |
| ♗ Bishop | Diagonal unlimited | Clear path required |
| ♕ Queen | Rook + Bishop combined | Most powerful piece |
| ♔ King | 1 square any direction | Cannot move into check |

### Not Implemented
- ❌ Castling
- ❌ En passant
- ❌ Pawn promotion
- ❌ Stalemate detection
- ❌ Draw by repetition

## 🛡️ Security Notes

**Current implementation:**
- Moves validated on both sides (anti-cheat)
- No encryption (local network recommended)
- No authentication (trust-based)

**For production use, add:**
- TLS/SSL encryption
- Player authentication
- Message signing
- Rate limiting

## 🐛 Known Limitations

- Requires Windows (no Linux/macOS support)
- .NET Framework 4.8 dependency
- Exactly 2 players required (no AI)
- No game history or replay
- No save/load functionality

## 🎯 Testing

Run through the [TESTING_CHECKLIST.md](TESTING_CHECKLIST.md) for comprehensive validation:
- 22 functional tests
- Build verification
- Network resilience tests
- Edge case handling

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📝 License

This project is open source and available under standard terms.

## 👤 Author

**lgp1012**
- GitHub: [@lgp1012](https://github.com/lgp1012)

## 🙏 Acknowledgments

- Built with C# and Windows Forms
- TCP protocol for reliable communication and server relay

---

**Enjoy playing chess! ♔♕♖♗♘♙**

For questions or issues, please open a GitHub issue or refer to the documentation files.
