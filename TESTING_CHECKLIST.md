# Testing Checklist

## Pre-Testing Setup

- [ ] Windows OS (10 or 11)
- [ ] Visual Studio 2019 or later installed
- [ ] .NET Framework 4.8 Developer Pack installed
- [ ] Solution opens without errors in Visual Studio

## Build Tests

### Server Build
- [ ] Right-click `ChessGame` project → Build
- [ ] Build succeeds without errors
- [ ] Build output shows: `Build succeeded`
- [ ] `ChessGame/bin/Debug/ChessGame.exe` exists

### Client Build
- [ ] Right-click `Client` project → Build
- [ ] Build succeeds without errors
- [ ] Build output shows: `Build succeeded`
- [ ] `Client/bin/Debug/Client.exe` exists
- [ ] No warnings about missing UdpGameClient.cs

## Functional Tests

### Test 1: Server Startup
- [ ] Run `ChessGame.exe`
- [ ] Server form appears
- [ ] Click "Start Server" button
- [ ] Status shows "Server started on port 5000"
- [ ] No errors in console/log

### Test 2: Single Client Connection
- [ ] With server running, run `Client.exe`
- [ ] PlayerNameForm appears
- [ ] Enter name (e.g., "Alice")
- [ ] Enter server IP "127.0.0.1"
- [ ] Click OK
- [ ] ClientForm appears with connection UI
- [ ] Click "Connect" button
- [ ] Status changes to "Connected"
- [ ] Server shows: "Alice (127.0.0.1) connected"

### Test 3: Two Client Matching
- [ ] Run second `Client.exe` instance
- [ ] Enter different name (e.g., "Bob")
- [ ] Enter server IP "127.0.0.1"
- [ ] Click OK and Connect
- [ ] Server shows: "Bob (127.0.0.1) connected"
- [ ] Server shows: "Match started between Alice and Bob"
- [ ] Both clients show UDP initialization messages
- [ ] Both clients show: "Match started!"
- [ ] ChessGameForm opens for both players

### Test 4: UDP Connection
- [ ] Check Alice's client log: `[UDP] Initialized on port XXXXX`
- [ ] Check Bob's client log: `[UDP] Initialized on port YYYYY`
- [ ] Check Alice's client log: `[UDP] Connected to opponent at 127.0.0.1:YYYYY`
- [ ] Check Bob's client log: `[UDP] Connected to opponent at 127.0.0.1:XXXXX`
- [ ] No UDP errors shown

### Test 5: Chess Board Display
- [ ] Both players see 8x8 board
- [ ] Board has Beige and Brown squares (NOT White/Crimson)
- [ ] All pieces display with Unicode symbols:
  - [ ] White King: ♔
  - [ ] White Queen: ♕
  - [ ] White Rook: ♖
  - [ ] White Bishop: ♗
  - [ ] White Knight: ♘
  - [ ] White Pawn: ♙
  - [ ] Black King: ♚
  - [ ] Black Queen: ♛
  - [ ] Black Rook: ♜
  - [ ] Black Bishop: ♝
  - [ ] Black Knight: ♞
  - [ ] Black Pawn: ♟
- [ ] White pieces at bottom for White player
- [ ] Black pieces at bottom for Black player
- [ ] Turn indicator shows "Your Turn" for White player
- [ ] Turn indicator shows "Bob's Turn" for Black player (or opponent name)

### Test 6: Piece Selection (White Player)
- [ ] Click on white pawn (e.g., e2)
- [ ] Pawn square turns Gold (selected)
- [ ] Valid moves highlight in LightGreen (e3, e4)
- [ ] Click same pawn again deselects (colors reset)
- [ ] Click different white piece selects it
- [ ] Cannot select black pieces (opponent's)

### Test 7: Valid Pawn Move
- [ ] White player clicks pawn at e2
- [ ] Valid moves (e3, e4) show in LightGreen
- [ ] Click e4 destination
- [ ] Pawn moves from e2 to e4 on White's board
- [ ] Pawn moves from e2 to e4 on Black's board (via UDP)
- [ ] Turn changes to "Bob's Turn" on White's board
- [ ] Turn changes to "Your Turn" on Black's board
- [ ] No errors in either client

### Test 8: Black's Response
- [ ] Black player clicks pawn at e7
- [ ] Pawn highlights in Gold
- [ ] Valid moves (e6, e5) show in LightGreen
- [ ] Click e5
- [ ] Move appears on both boards
- [ ] Turn switches back to White

### Test 9: Invalid Move Prevention
- [ ] White player tries to move pawn backward
  - [ ] Move is NOT highlighted (not in valid moves)
  - [ ] Cannot execute the move
- [ ] Try to move opponent's piece
  - [ ] Piece doesn't select (Gold highlight)
  - [ ] No move occurs
- [ ] Try to move off turn
  - [ ] Clicks have no effect during opponent's turn

### Test 10: Capture Move
- [ ] Play several moves to get pieces in position
- [ ] Move white piece to attack black piece
- [ ] Enemy square highlights in IndianRed (capturable)
- [ ] Click enemy square
- [ ] Enemy piece disappears
- [ ] Attacking piece moves to that square
- [ ] Move appears correctly on both boards

### Test 11: Knight Movement
- [ ] Click white knight at b1
- [ ] Valid L-shaped moves highlight (a3, c3)
- [ ] Move knight to c3
- [ ] Knight jumps over pieces correctly
- [ ] Move appears on both boards

### Test 12: Bishop Movement
- [ ] Move pawn to open diagonal
- [ ] Click bishop
- [ ] Diagonal squares highlight
- [ ] Blocked diagonal squares DON'T highlight
- [ ] Move bishop along diagonal
- [ ] Works correctly

### Test 13: Check Detection
- [ ] Play moves to put opponent in check
- [ ] Continue playing (check detection passive for now)
- [ ] Game continues normally

### Test 14: Checkmate Detection (Simple)
- [ ] Play Scholar's Mate sequence:
  - [ ] 1. e4 e5
  - [ ] 2. Bc4 Nc6
  - [ ] 3. Qh5 Nf6
  - [ ] 4. Qxf7#
- [ ] MessageBox appears: "Checkmate! Bạn đã thắng!" (Winner)
- [ ] MessageBox appears: "Đối thủ đã chiếu bí bạn!" (Loser)
- [ ] Game recognizes mate correctly

### Test 15: Pause Functionality
- [ ] Click "Pause" button during game
- [ ] Dialog asks "Tiếp tục ván đấu?"
- [ ] Click "No" to pause
- [ ] Turn indicator shows "Game Paused"
- [ ] Opponent sees: "Alice đã tạm dừng ván đấu"
- [ ] Opponent's board disabled during pause

### Test 16: Exit Functionality
- [ ] Click "Exit" button
- [ ] Dialog asks "Bạn chắc chắn muốn thoát?"
- [ ] Click "Yes"
- [ ] Game closes for exiting player
- [ ] Opponent sees: "Alice đã thoát ván đấu"
- [ ] Opponent's game closes
- [ ] Both return to ClientForm

### Test 17: Server Shutdown During Game
- [ ] Start a game with 2 players
- [ ] On server, click "Stop Server"
- [ ] Both clients receive shutdown message
- [ ] Both game forms close gracefully
- [ ] Both clients return to ClientForm
- [ ] No crashes or errors

### Test 18: Network Resilience
- [ ] Start game between 2 clients
- [ ] Make several moves successfully
- [ ] Verify UDP messages still flowing
- [ ] No packet loss visible (all moves appear)

## Performance Tests

### Test 19: Move Speed
- [ ] Make 10 rapid moves
- [ ] Each move appears on opponent screen within 100ms
- [ ] No lag or delay noticeable
- [ ] UDP faster than previous TCP-only version

### Test 20: Multiple Games
- [ ] Close first pair of clients
- [ ] Connect new pair
- [ ] Server creates new match
- [ ] Game works normally
- [ ] Can repeat 3-4 times without issues

## Edge Cases

### Test 21: Disconnect Handling
- [ ] Start game
- [ ] Close one client abruptly (Alt+F4)
- [ ] Other client shows disconnect message
- [ ] No crash or hang

### Test 22: Invalid Server IP
- [ ] Enter invalid IP (e.g., "999.999.999.999")
- [ ] Click Connect
- [ ] Error message appears
- [ ] Client doesn't crash
- [ ] Can retry with correct IP

## Bug Tracking

| Test # | Status | Notes |
|--------|--------|-------|
| 1      | ⬜     |       |
| 2      | ⬜     |       |
| 3      | ⬜     |       |
| 4      | ⬜     |       |
| 5      | ⬜     |       |
| 6      | ⬜     |       |
| 7      | ⬜     |       |
| 8      | ⬜     |       |
| 9      | ⬜     |       |
| 10     | ⬜     |       |
| 11     | ⬜     |       |
| 12     | ⬜     |       |
| 13     | ⬜     |       |
| 14     | ⬜     |       |
| 15     | ⬜     |       |
| 16     | ⬜     |       |
| 17     | ⬜     |       |
| 18     | ⬜     |       |
| 19     | ⬜     |       |
| 20     | ⬜     |       |
| 21     | ⬜     |       |
| 22     | ⬜     |       |

**Legend:** ⬜ Not tested | ✅ Pass | ❌ Fail | ⚠️ Issue found

## Known Limitations (Not Bugs)

- No castling implemented
- No en passant implemented
- No pawn promotion (pawn reaching end)
- No stalemate detection
- No draw by repetition
- No move timer/clock
- UDP port assignment is random (not configurable)
- Game requires exactly 2 players (no AI)

## Success Criteria

✅ All tests 1-18 must pass
✅ At least 15/22 tests should pass
✅ No critical crashes or data corruption
✅ UDP communication works reliably
✅ Chess rules enforced correctly
✅ UI updates properly on both sides
