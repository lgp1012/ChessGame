# Chess Game Architecture

## System Overview

```
┌─────────────────┐         TCP          ┌─────────────────┐
│   Client A      │ ◄──────────────────► │   TCP Server    │
│  (White Player) │     Port 5000        │  (ChessGame)    │
└─────────────────┘                      └─────────────────┘
        │                                         ▲
        │                                         │
        │ UDP (Direct P2P)                       │ TCP
        │ Dynamic Port                            │
        │                                         │
        ▼                                         ▼
┌─────────────────┐                      ┌─────────────────┐
│   UDP Client    │                      │   Client B      │
│    (White)      │ ◄──────────────────► │ (Black Player)  │
└─────────────────┘     UDP Direct       └─────────────────┘
                       Dynamic Port
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
   │◄───Match started!────┤────Match started!──►│
   │                      │                     │
```

### Phase 2: UDP Setup
```
Client A                Server              Client B
   │                      │                     │
   ├──Initialize UDP──┐   │                     │
   │   (Port 12345)   │   │   ┌──Initialize UDP─┤
   │                  │   │   │  (Port 54321)   │
   ├─[UDP_PORT]12345──────►   │                 │
   │                      ◄────[UDP_PORT]54321──┤
   │                      │                     │
   │◄[UDP_INFO]IP_B|54321─┤                     │
   │                      ├─[UDP_INFO]IP_A|12345►│
   │                      │                     │
   │──────Connect UDP─────┼──────────────────────►│
   │                      │                     │
```

### Phase 3: Gameplay
```
Client A                                    Client B
   │                                           │
   ├──User clicks piece──┐                    │
   │  (validates move)   │                    │
   │                     │                    │
   ├──UDP: "3,4->3,5"────────────────────────►│
   │                                          │
   │                                   ┌──Update board
   │                                   │  (opponent's move)
   │                                   │
   │◄─────────UDP: "6,4->5,4"──────────────────┤
   │                                          │
   └──Update board                            │
      (opponent's move)                       │
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

### UdpGameClient.cs
- Peer-to-peer move transmission
- Asynchronous message receiving
- Event-based communication

### TcpServer.cs
- Client matchmaking (2 players)
- Color assignment
- UDP endpoint exchange
- Match coordination

### ClientForm.cs
- Server connection management
- UDP client initialization
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
   └─> udpClient.SendMove(r1,c1,r2,c2)
   └─> UpdateBoardDisplay()
   └─> Check for checkmate
       └─> If checkmate: SendMessage("[CHECKMATE]")
```

### Move Reception
```
1. UdpClient.ReceiveAsync() gets data
   └─> Parse "r1,c1->r2,c2"
   └─> Invoke OnMoveReceived event
       └─> ChessGameForm.UdpClient_OnMoveReceived()
           └─> chessBoard.MovePiece(r1,c1,r2,c2)
           └─> UpdateBoardDisplay()
           └─> Check for checkmate
           └─> Switch turn to player
```

## Message Protocol

### TCP Messages (Reliable)
| Message | Direction | Purpose |
|---------|-----------|---------|
| `PlayerName` | Client → Server | Initial identification |
| `[OPPONENT]\|Name\|Color` | Server → Client | Match pairing |
| `Match started!` | Server → Both | Game begin signal |
| `[UDP_PORT]port` | Client → Server | Share UDP port |
| `[UDP_INFO]ip\|port` | Server → Client | Opponent endpoint |
| `[PAUSE]Name` | Any → All | Game pause |
| `[EXIT]Name` | Any → All | Player quit |

### UDP Messages (Fast)
| Message | Direction | Purpose |
|---------|-----------|---------|
| `r1,c1->r2,c2` | Client ↔ Client | Chess move |
| `[CHECKMATE]` | Winner → Loser | Game over |

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
✅ UDP peer-to-peer communication
✅ TCP server coordination
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
