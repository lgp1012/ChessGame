# Chess Game Architecture

## System Overview

```
┌─────────────────┐         TCP          ┌─────────────────┐
│   Client A      │ ◄──────────────────► │   TCP Server    │
│  (White Player) │     Port 5000        │  (Relay)        │
└─────────────────┘                      └─────────────────┘
                                                  │
                                                  │ TCP
                                                  │ (Relay)
                                                  │
                                                  ▼
                                          ┌─────────────────┐
                                          │   Client B      │
                                          │ (Black Player)  │
                                          └─────────────────┘
```

## Communication Flow

### Phase 1: Initial Connection (TCP)
```
Client A                Server              Client B
   │                      │                     │
   ├──[PlayerName]────────►                     │
   │                      │                     │
   │                      ◄────[PlayerName]─────┤
   │                      │                     │
   │◄──[OPPONENT|B|WHITE]─┤                     │
   │                      ├─[OPPONENT|A|BLACK]─►│
   │                      │                     │
```

### Phase 2: Gameplay (TCP Relay)
```
Client A                Server              Client B
   │                      │                     │
   ├──User clicks piece──┐                     │
   │  (validates move)   │                     │
   │                     │                     │
   ├──TCP: [MOVE]3,4->3,5────────►             │
   │                      │                     │
   │                      ├─TCP: [MOVE]3,4->3,5►│
   │                      │                     │
   │                      │              ┌──Update board
   │                      │              │  (opponent's move)
   │                      │              │
   │                      ◄──TCP: [MOVE]6,4->5,4┤
   │                      │                     │
   │◄─TCP: [MOVE]6,4->5,4─┤                     │
   │                      │                     │
   └──Update board                              │
      (opponent's move)                         │
```

## Component Responsibilities

### ChessBoard.cs
- Board state management (8x8 grid)
- Piece movement validation
- Check/checkmate detection
- Game rules enforcement

### ChessGameForm.cs
- Interactive UI (Button grid)
- Piece selection and highlighting
- Move validation before sending
- Display updates on move received

### TcpServer.cs
- Client matchmaking (2 players)
- Color assignment
- Message relay between clients
- Match coordination

### ClientForm.cs
- Server connection management
- Message routing
- Game lifecycle management

## Data Flow

### Move Execution
```
1. User clicks piece
   └─> ChessGameForm.CellClick(row, col)
       └─> ChessBoard.IsValidMoveSafe(r1,c1,r2,c2)
           ├─> IsValidMove() - Basic rules
           └─> MoveLeavesKingInCheck() - Safety check
               
2. If valid:
   └─> chessBoard.MovePiece(r1,c1,r2,c2)
   └─> OnGameMessage("[MOVE]r1,c1->r2,c2") - Send via TCP
   └─> UpdateBoardDisplay()
   └─> Check for checkmate
       └─> If checkmate: OnGameMessage("[CHECKMATE]")
```

### Move Reception
```
1. TcpServer receives [MOVE] message
   └─> BroadcastMessage() - Relay to opponent
       └─> ClientForm.TcpConnection_OnMessageReceived()
           └─> ChessGameForm.HandleOpponentMove()
               └─> Parse "[MOVE]r1,c1->r2,c2"
               └─> chessBoard.MovePiece(r1,c1,r2,c2)
               └─> UpdateBoardDisplay()
               └─> Check for checkmate
               └─> Switch turn to player
```

## Message Protocol

### TCP Messages (Client ↔ Server ↔ Client)
| Message | Direction | Purpose |
|---------|-----------|---------|
| `PlayerName` | Client → Server | Initial identification |
| `[OPPONENT]\|Name\|Color` | Server → Client | Match pairing |
| `[MOVE]r1,c1->r2,c2` | Client → Server → Client | Chess move relay |
| `[CHECKMATE]` | Client → Server → Client | Game over |
| `[PAUSE]Name` | Client → Server → Client | Game pause |
| `[RESUME]Name` | Client → Server → Client | Game resume |
| `[EXIT]Name` | Client → Server → Client | Player quit |
| `[STOPMATCH]` | Server → Clients | Server stopped match |

## UI Layout

### ChessGameForm
```
┌─────────────────────────────────────────┐
│ Chess Game - PlayerName           [X]   │
├─────────────────────────────────────────┤
│  Your Turn                [Pause] [Exit]│
├─────────────────────────────────────────┤
│  ┌─────────────────────────────────┐    │
│  │ ♜ ♞ ♝ ♛ ♚ ♝ ♞ ♜  ← Black        │    │
│  │ ♟ ♟ ♟ ♟ ♟ ♟ ♟ ♟                │    │
│  │ □ ■ □ ■ □ ■ □ ■                │    │
│  │ ■ □ ■ □ ■ □ ■ □                │    │
│  │ □ ■ □ ■ □ ■ □ ■                │    │
│  │ ■ □ ■ □ ■ □ ■ □                │    │
│  │ ♙ ♙ ♙ ♙ ♙ ♙ ♙ ♙                │    │
│  │ ♖ ♘ ♗ ♕ ♔ ♗ ♘ ♖  ← White        │    │
│  └─────────────────────────────────┘    │
│  Legend:                                │
│  □/■ = Beige/Brown squares             │
│  🟢 = Valid empty move                 │
│  🔴 = Can capture                      │
│  🟡 = Selected piece                   │
└─────────────────────────────────────────┘
```

## Key Features

✅ Complete chess move validation
✅ Check and checkmate detection  
✅ Interactive button-based UI
✅ Real-time move highlighting
✅ TCP server with message relay
✅ Color-coded piece display
✅ Turn-based gameplay enforcement

## Building & Running

**Requirements:** Windows + .NET Framework 4.8

**Steps:**
1. Build Server project (ChessGame)
2. Build Client project (Client) 
3. Run Server, click "Start Server"
4. Run 2x Client instances
5. Connect both to server
6. Play chess!

See IMPLEMENTATION_NOTES.md for detailed documentation.
