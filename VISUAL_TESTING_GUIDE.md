# Visual Testing Guide for Match State and Pause/Resume Features

## Test Setup
1. Build both projects in Visual Studio (requires Windows + .NET Framework 4.8)
2. Start the Server application (ChessGame.exe)
3. Start two Client applications (Client.exe) on same or different machines

---

## Test 1: Server Auto-Reset After Disconnection

### Steps:
1. **Server:** Click "Start Server"
2. **Client 1 & 2:** Enter names and connect
3. **Server:** Click "Start Match" and wait for countdown
4. **Server:** Verify `lblMatchStatus` shows "Match started!"
5. **Client 1 & 2:** Close both client applications
6. **Server:** Verify message log shows both disconnections
7. **Server:** ✅ Verify `lblMatchStatus` changes to "Waiting for players..."
8. **Server:** ✅ Verify log shows "[HH:mm:ss] Match auto-reset: not enough players"

### Expected UI States:

**Before Disconnect:**
```
Server UI:
  lblMatchStatus: "Match started!"
  btnStartMatch: Disabled
  btnStopMatch: Enabled
```

**After Both Disconnect:**
```
Server UI:
  lblMatchStatus: "Waiting for players..."
  btnStartMatch: Disabled (need 2 clients)
  btnStopMatch: Disabled
  Message Log: "[HH:mm:ss] Match auto-reset: not enough players"
```

---

## Test 2: Server Stop Match Notification

### Steps:
1. **Server:** Start server and match with 2 clients
2. **Both Clients:** Verify chess game form is open
3. **Server:** Click "Stop Match" button
4. **Both Clients:** ✅ Verify MessageBox appears: "Server đã dừng trận đấu. Bạn sẽ quay về form kết nối."
5. **Both Clients:** ✅ Click OK and verify chess game closes
6. **Both Clients:** ✅ Verify ClientForm (lobby) is visible again

### Expected UI States:

**Client After Stop:**
```
MessageBox:
  Title: "Trận đấu kết thúc"
  Message: "Server đã dừng trận đấu. Bạn sẽ quay về form kết nối."
  Button: OK
  Icon: Information (blue i)

After OK:
  - ChessGameForm closed
  - ClientForm visible
  - Status: "Connected"
```

---

## Test 3: Pause/Resume with Dialog

### Test 3A: Successful Pause
**Steps:**
1. **Client 1:** Click "Pause" button
2. **Client 1:** ✅ Verify dialog appears with title "Pause"
3. **Client 1:** ✅ Verify message: "Bạn muốn tạm dừng ván đấu?"
4. **Client 1:** ✅ Verify buttons: "Yes" and "No"
5. **Client 1:** Click "Yes"
6. **Client 1:** ✅ Verify button text changes to "Resume"
7. **Client 1:** ✅ Verify button color changes to Green
8. **Client 1:** ✅ Verify lblTurn shows "Game Paused" in Red

### Expected UI States:

**Before Pause:**
```
Client 1:
  btnPause: "Pause" (Orange)
  lblTurn: "Your Turn" (Green) or "[Name]'s Turn" (Blue)
```

**After Pause:**
```
Client 1:
  btnPause: "Resume" (Green)
  lblTurn: "Game Paused" (Red)
```

### Test 3B: Cancel Pause
**Steps:**
1. **Client 1:** Click "Pause" button
2. **Client 1:** Click "No" in dialog
3. **Client 1:** ✅ Verify button stays "Pause" (Orange)
4. **Client 1:** ✅ Verify game continues normally

### Test 3C: Resume
**Steps:**
1. **Client 1:** (After successful pause) Click "Resume" button
2. **Client 1:** ✅ Verify button text changes to "Pause"
3. **Client 1:** ✅ Verify button color changes to Orange
4. **Client 1:** ✅ Verify lblTurn shows turn status (not "Game Paused")

---

## Test 4: Opponent Pause Blocking

### Steps:
1. **Client 1:** Click "Pause" and confirm "Yes"
2. **Client 2:** ✅ Verify semi-transparent black overlay appears over chess board
3. **Client 2:** ✅ Verify overlay text shows: "[Client 1 Name] đã tạm dừng trận đấu"
4. **Client 2:** ✅ Try clicking on chess pieces - verify NO interaction possible
5. **Client 2:** ✅ Verify Pause and Exit buttons are still visible and clickable
6. **Client 1:** Click "Resume"
7. **Client 2:** ✅ Verify overlay disappears immediately
8. **Client 2:** ✅ Try clicking on chess pieces - verify interaction restored

### Expected Visual States:

**Client 2 When Opponent Pauses:**
```
Chess Board Area:
  - Semi-transparent black overlay (75% opacity)
  - White text centered: "[Opponent Name] đã tạm dừng trận đấu"
  - Font: Bold, 18pt
  - All chess buttons disabled (cannot click)

Top Controls:
  - lblTurn: Still visible
  - btnPause: Still visible and clickable (Orange)
  - btnExit: Still visible and clickable (Red)
```

**Client 2 When Opponent Resumes:**
```
Chess Board Area:
  - Overlay completely hidden
  - All chess buttons enabled (can click)
  - Normal board appearance
```

---

## Test 5: Server Logging

### Steps:
1. **Server:** Monitor the message list box during gameplay
2. **Client 1:** Pause the game
3. **Server:** ✅ Verify log shows: "[HH:mm:ss] [Client 1 Name] đã tạm dừng ván đấu"
4. **Server:** ✅ Verify lblMatchStatus shows: "Match is paused by [Client 1 Name]"
5. **Client 1:** Resume the game
6. **Server:** ✅ Verify log shows: "[HH:mm:ss] [Client 1 Name] đã tiếp tục ván đấu"
7. **Server:** ✅ Verify lblMatchStatus shows: "Match started!"
8. **Client 1:** Exit the game
9. **Server:** ✅ Verify log shows: "[HH:mm:ss] [Client 1 Name] đã thoát ván đấu"

### Expected Server Message Log:
```
[10:30:45] Server started
[10:30:50] Player1 connected from 192.168.1.100
[10:30:52] Player2 connected from 192.168.1.101
[10:30:55] Colors assigned: Player1 (WHITE) vs Player2 (BLACK)
[10:31:00] Match countdown started
[10:31:05] Match started!
[10:31:20] Player1 đã tạm dừng ván đấu
[10:31:35] Player1 đã tiếp tục ván đấu
[10:32:00] Player1 đã thoát ván đấu
```

### Expected Server lblMatchStatus Changes:
```
Initial:           "Waiting for players..."
After 2 Connect:   "Waiting for players..."
Countdown:         "Match starts in X seconds..."
Match Started:     "Match started!"
Client Pauses:     "Match is paused by [Client Name]"
Client Resumes:    "Match started!"
Server Stops:      "Match paused by server"
Clients Leave:     "Waiting for players..."
```

---

## Test 6: Edge Cases

### Test 6A: Both Players Pause Simultaneously
**Steps:**
1. **Client 1 & 2:** Click Pause at nearly same time
2. ✅ Verify each client shows their own pause state correctly
3. ✅ Verify each client sees opponent's pause overlay

### Test 6B: Server Stop During Pause
**Steps:**
1. **Client 1:** Pause the game
2. **Server:** Click "Stop Match"
3. ✅ Verify both clients receive stop message
4. ✅ Verify both clients close properly even with pause overlay visible

### Test 6C: Client Disconnect During Pause
**Steps:**
1. **Client 1:** Pause the game
2. **Client 1:** Close client application
3. **Server:** ✅ Verify shows disconnect message
4. **Server:** ✅ Verify match auto-resets
5. **Client 2:** ✅ Verify receives opponent exit notification

---

## UI Element Reference

### Server Form Elements
- `lblServerStatus` - "Server status: RUNNING" / "STOPPED"
- `lblConnectClient` - "Connected clients: X / 2"
- `lblMatchStatus` - Match state indicator
- `btnStartServer` - Start TCP server
- `btnEndServer` - Stop TCP server
- `btnStartMatch` - Begin countdown and match
- `btnStopMatch` - Stop current match
- `msgServer` - ListBox showing all events and messages

### Client Game Form Elements
- `lblTurn` - Shows whose turn it is or "Game Paused"
- `btnPause` - "Pause" (Orange) or "Resume" (Green)
- `btnExit` - "Exit" (Red)
- `pauseOverlay` - Panel blocking board during opponent pause
- `pauseLabel` - Text shown in overlay
- `cells[8,8]` - Chess board buttons

---

## Color Coding Reference

### Button States:
- **Pause (Orange)** - Can pause the game
- **Resume (Green)** - Can resume from your pause
- **Exit (Red)** - Exit the game

### Label Colors:
- **Green** - Your turn
- **Blue** - Opponent's turn
- **Red** - Game paused

### Overlay:
- **Semi-transparent Black (75%)** - Opponent paused
- **White Text** - Pause message

---

## Common Issues to Check

### ❌ Overlay doesn't appear
- Check if `pauseOverlay.Visible` is set to true
- Verify overlay is positioned correctly over board
- Check z-order (should be brought to front)

### ❌ Buttons still clickable when opponent pauses
- Verify `btn.Enabled = false` is called for all cells
- Check that overlay is covering the chess board area

### ❌ Match doesn't reset after disconnection
- Verify `OnMatchShouldReset` event is wired up
- Check if event handler is on UI thread (InvokeRequired)
- Verify client count is checked correctly

### ❌ Server doesn't log pause/resume
- Verify event handlers are registered in StartServer
- Check if events are unregistered in StopServer
- Verify InvokeRequired pattern for thread safety

---

## Screenshots to Capture

1. **Server before match** - Waiting for players
2. **Server during match** - Match started
3. **Server when client pauses** - Match is paused by [name]
4. **Server after disconnect** - Auto-reset message
5. **Client pause dialog** - "Bạn muốn tạm dừng ván đấu?"
6. **Client with Resume button** - Green Resume button
7. **Client with pause overlay** - Opponent paused screen
8. **Server stop match notification** - Client MessageBox
9. **Server message log** - Full event history

These screenshots can be used to verify all features are working correctly!
