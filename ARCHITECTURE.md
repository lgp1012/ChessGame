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

