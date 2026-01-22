# Chess Game with TCP/UDP Communication

A complete multiplayer chess game built with C# .NET Framework 4.8, featuring real-time gameplay over UDP for fast move transmission and TCP for reliable server coordination.

### Interactive UI
- 🎨 Button-based chessboard with **Beige & Brown** squares
- 🎭 Unicode chess symbols: ♔♕♖♗♘♙ (White) & ♚♛♜♝♞♟ (Black)
- 🎯 Move highlighting:
  - **Gold**: Selected piece
  - **Light Green**: Valid empty destination
  - **Indian Red**: Capturable enemy piece
- 🖱️ Click-to-move interface with visual feedback

### Network Architecture
- 🌐 **TCP Server**: Matchmaking and coordination
- ⚡ **UDP Peer-to-Peer**: Fast, direct move transmission
- 🔄 **Hybrid Protocol**: TCP for control, UDP for gameplay
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
   - Ensure both `Client` and `Server` build successfully

4. **Start the server**
   - Run `Server` project
   - Click **"Start Server"** button
   - Server listens on **port 5000**

5. **Connect clients**
   - Run `Client` project (start 2 instances)
   - Enter player name
   - Enter server IP: IP's Server
   - Click **"Connect"**

6. **Play chess!**
   - Server start-matches when 2 clients connect
   - White player goes first
   - Click pieces and valid squares to move

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
│   (White)   │      Port 5000           │ (Matching)  │
└─────────────┘                          └─────────────┘
       │                                        │
       │ UDP Direct (Dynamic Port)             │ TCP
       ▼                                        ▼
┌─────────────┐                          ┌─────────────┐
│ UdpClient A │ ◄──────────────────────► │  Client B   │
└─────────────┘      UDP P2P             │   (Black)   │
                                          └─────────────┘
```

### Communication Flow
1. **TCP**: Initial connection, matchmaking, opponent info
2. **UDP Setup**: Server exchanges UDP endpoints
3. **UDP Gameplay**: Direct peer-to-peer move transmission
4. **TCP Control**: Pause, exit, server commands

## 🧩 Project Structure

```
ChessGame/
├── Client/                         # Client Application
│   ├── ChessBoard.cs              # Chess logic & validation
│   ├── ChessGameForm.cs           # Game UI (button board)
│   ├── UdpGameClient.cs           # UDP communication
│   ├── ClientForm.cs              # Connection UI
│   ├── TcpClient.cs               # TCP server connection
│   └── Client.csproj              # Client project
├── ChessGame(Server)/                      # Server Application
   ├── TcpServer.cs               # TCP server & UDP coordination
   ├── ServerForm.cs              # Server UI
   └── Server.csproj              # Server project

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

### UdpGameClient.cs
Peer-to-peer game communication:
- Dynamic port assignment
- Asynchronous message receiving
- Event-based architecture
- Move serialization (format: `r1,c1->r2,c2`)

### TcpServer.cs
Server coordination:
- Client matchmaking (pairs of 2)
- Color assignment (White/Black)
- UDP endpoint exchange
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

## 👤 Author

Member:
- Lưu Gia Phúc - GitHub: [@lgp1012](https://github.com/lgp1012)
- Nguyễn Huỳnh Nghĩa Nhân

## 🙏 Acknowledgments

- Built with C# and Windows Forms
- UDP protocol for real-time gameplay
- TCP for reliable server coordination

---

**Enjoy playing chess! ♔♕♖♗♘♙**

For questions or issues, please open a GitHub issue or refer to the documentation files.
