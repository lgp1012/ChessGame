# Visual Guide: Chess Game UI

## Expected Game Board Appearance

### Full Window Layout
```
┌────────────────────────────────────────────────────────┐
│  Chess Game - Alice                            [_][□][X] │
├────────────────────────────────────────────────────────┤
│                                                         │
│  Your Turn ✓               [Pause]  [Exit]             │
│                                                         │
│  ┌──────────────────────────────────────────────┐      │
│  │  A    B    C    D    E    F    G    H        │      │
│  │ ┌────┬────┬────┬────┬────┬────┬────┬────┐ 8 │      │
│  │ │ ♜ │ ♞ │ ♝ │ ♛ │ ♚ │ ♝ │ ♞ │ ♜ │   │      │
│  │ ├────┼────┼────┼────┼────┼────┼────┼────┤ 7 │      │
│  │ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │   │      │
│  │ ├────┼────┼────┼────┼────┼────┼────┼────┤ 6 │      │
│  │ │    │    │    │    │    │    │    │    │   │      │
│  │ ├────┼────┼────┼────┼────┼────┼────┼────┤ 5 │      │
│  │ │    │    │    │    │    │    │    │    │   │      │
│  │ ├────┼────┼────┼────┼────┼────┼────┼────┤ 4 │      │
│  │ │    │    │    │ 🟢 │    │    │    │    │   │      │
│  │ ├────┼────┼────┼────┼────┼────┼────┼────┤ 3 │      │
│  │ │    │    │    │ 🟢 │    │    │    │    │   │      │
│  │ ├────┼────┼────┼────┼────┼────┼────┼────┤ 2 │      │
│  │ │ ♙ │ ♙ │ ♙ │ 🟡 │ ♙ │ ♙ │ ♙ │ ♙ │   │      │
│  │ ├────┼────┼────┼────┼────┼────┼────┼────┤ 1 │      │
│  │ │ ♖ │ ♘ │ ♗ │ ♕ │ ♔ │ ♗ │ ♘ │ ♖ │   │      │
│  │ └────┴────┴────┴────┴────┴────┴────┴────┘   │      │
│  └──────────────────────────────────────────────┘      │
│                                                         │
└────────────────────────────────────────────────────────┘

Legend:
🟡 = Selected piece (Gold background)
🟢 = Valid empty move (LightGreen background)
🔴 = Can capture enemy (IndianRed background)
Light squares = Beige (#F5F5DC)
Dark squares = Brown (#A52A2A)
```

## Color Scheme Details

### Board Colors
```css
Beige Square:  #F5F5DC  ░░░░░  (Light squares)
Brown Square:  #A52A2A  ████  (Dark squares)
```

### Highlight Colors
```css
Gold:         #FFD700  ████  Selected piece
LightGreen:   #90EE90  ████  Valid empty destination
IndianRed:    #CD5C5C  ████  Capturable enemy piece
```

## Chess Piece Symbols

### White Pieces (Color: White #FFFFFF)
```
♔  King    (Vua)
♕  Queen   (Hậu)
♖  Rook    (Xe)
♗  Bishop  (Tượng)
♘  Knight  (Mã)
♙  Pawn    (Tốt)
```

### Black Pieces (Color: Black #000000)
```
♚  King    (Vua)
♛  Queen   (Hậu)
♜  Rook    (Xe)
♝  Bishop  (Tượng)
♞  Knight  (Mã)
♟  Pawn    (Tốt)
```

## State Examples

### 1. Initial Board (Start of Game)
```
  A  B  C  D  E  F  G  H
8 ♜ ♞ ♝ ♛ ♚ ♝ ♞ ♜   Black
7 ♟ ♟ ♟ ♟ ♟ ♟ ♟ ♟
6 ░ ■ ░ ■ ░ ■ ░ ■   (empty)
5 ■ ░ ■ ░ ■ ░ ■ ░   (empty)
4 ░ ■ ░ ■ ░ ■ ░ ■   (empty)
3 ■ ░ ■ ░ ■ ░ ■ ░   (empty)
2 ♙ ♙ ♙ ♙ ♙ ♙ ♙ ♙
1 ♖ ♘ ♗ ♕ ♔ ♗ ♘ ♖   White

Turn: Your Turn (White)
```

### 2. Piece Selected (E2 Pawn)
```
  A  B  C  D  E  F  G  H
8 ♜ ♞ ♝ ♛ ♚ ♝ ♞ ♜
7 ♟ ♟ ♟ ♟ ♟ ♟ ♟ ♟
6 ░ ■ ░ ■ ░ ■ ░ ■
5 ■ ░ ■ ░ ■ ░ ■ ░
4 ░ ■ ░ ■ 🟢 ■ ░ ■   <- E4 highlighted green (valid)
3 ■ ░ ■ ░ 🟢 ░ ■ ░   <- E3 highlighted green (valid)
2 ♙ ♙ ♙ ♙ 🟡 ♙ ♙ ♙   <- E2 highlighted gold (selected)
1 ♖ ♘ ♗ ♕ ♔ ♗ ♘ ♖

Action: Click E4 to move
```

### 3. After Move (E2→E4)
```
  A  B  C  D  E  F  G  H
8 ♜ ♞ ♝ ♛ ♚ ♝ ♞ ♜
7 ♟ ♟ ♟ ♟ ♟ ♟ ♟ ♟
6 ░ ■ ░ ■ ░ ■ ░ ■
5 ■ ░ ■ ░ ■ ░ ■ ░
4 ░ ■ ░ ■ ♙ ■ ░ ■   <- Pawn moved here
3 ■ ░ ■ ░ ░ ░ ■ ░
2 ♙ ♙ ♙ ♙ ░ ♙ ♙ ♙   <- Empty now
1 ♖ ♘ ♗ ♕ ♔ ♗ ♘ ♖

Turn: Bob's Turn (Black)
```

### 4. Capture Scenario (Knight can capture Pawn)
```
  A  B  C  D  E  F  G  H
8 ♜ ♞ ♝ ♛ ♚ ♝ ░ ♜
7 ♟ ♟ ♟ ♟ ░ ♟ ♟ ♟
6 ░ ■ ░ ■ ░ ■ ♞ ■   <- Black knight
5 ■ ░ ■ 🔴 🟢 ░ ■ ░   <- D5 red (can capture), E5 green (empty)
4 ░ ■ 🟢 ■ ♙ ■ ░ ■
3 ■ ░ ■ ♟ ░ 🔴 ■ ░   <- F3 red (can capture white pawn)
2 ♙ ♙ ♙ ♙ ░ ♙ ♙ ♙
1 ♖ ♘ ♗ ♕ ♔ ♗ ♘ ♖

Knight at G6 selected (gold)
Can capture: D5 pawn (red), F3 pawn (red)
Can move to: E5 (green), others...
```

## Turn Indicator States

### Your Turn (Active)
```
┌──────────────────────┐
│  Your Turn           │  <- Green text
│                      │
└──────────────────────┘
```

### Opponent's Turn
```
┌──────────────────────┐
│  Bob's Turn          │  <- Blue text
│                      │
└──────────────────────┘
```

### Game Paused
```
┌──────────────────────┐
│  Game Paused         │  <- Red text
│                      │
└──────────────────────┘
```

## Button States

### Normal Button
```
┌────────┐
│ Pause  │  Background: Orange, Text: White
└────────┘
```

### Exit Button
```
┌────────┐
│ Exit   │  Background: Red, Text: White
└────────┘
```

## Size Specifications

```
Window:      520px × 580px (8×60 + margins)
Square:      60px × 60px
Piece font:  24pt Arial
Turn label:  16pt Bold
Buttons:     110px × 35px
```

## Interaction Flow

### 1. Select Piece
```
Click ♙ at E2
  ↓
Square turns Gold 🟡
  ↓
Valid moves highlight Green 🟢/Red 🔴
```

### 2. Make Move
```
Click destination E4 (green)
  ↓
Piece moves: E2 → E4
  ↓
All highlights clear
  ↓
Turn switches to opponent
  ↓
UDP sends: "6,4->4,4"
```

### 3. Receive Opponent Move
```
UDP receives: "1,4->3,4"
  ↓
Board updates: E7 pawn → E5
  ↓
Turn switches to you
  ↓
"Your Turn" shows green
```

## Example Game States

### Opening Move (e4)
```
Before:                After:
2 ♙ ♙ ♙ ♙ ♙ ♙ ♙ ♙     2 ♙ ♙ ♙ ♙ ░ ♙ ♙ ♙
4 ░ ■ ░ ■ ░ ■ ░ ■     4 ░ ■ ░ ■ ♙ ■ ░ ■
```

### Scholar's Mate (Checkmate!)
```
  A  B  C  D  E  F  G  H
8 ♜ ♞ ♝ ♛ ♚ ░ ░ ♜
7 ♟ ♟ ♟ ♟ ░ ♕ ♟ ♟   <- White Queen on F7 (checkmate!)
6 ░ ■ ░ ■ ░ ■ ♞ ■
5 ■ ░ ♗ ░ ♟ ░ ■ ░
4 ░ ■ ░ ■ ♙ ■ ░ ■
3 ■ ░ ■ ░ ░ ♘ ■ ░
2 ♙ ♙ ♙ ♙ ░ ♙ ♙ ♙
1 ♖ ♘ ♗ ░ ♔ ░ ░ ♖

MessageBox: "Checkmate! Bạn đã thắng!"
```

## Network Status Display

### Client Form (Before Game)
```
┌─────────────────────────────────────┐
│ Chess Game Client - Alice     [X]   │
├─────────────────────────────────────┤
│                                     │
│ Status: Connected ✓    [Disconnect] │
│                                     │
│ Messages:                           │
│ ┌─────────────────────────────────┐ │
│ │ [10:15:23] Connected            │ │
│ │ [10:15:24] [Server] Waiting...  │ │
│ │ [10:15:30] [Server] Bob joined  │ │
│ │ [10:15:31] [Server] Match!      │ │
│ │ [10:15:31] [UDP] Port: 54321    │ │
│ │ [10:15:32] [UDP] Connected      │ │
│ └─────────────────────────────────┘ │
└─────────────────────────────────────┘
```

## Error States

### Invalid Move Attempt
```
Click ♙ at E4 → try to move back to E3
  ↓
No highlight (not valid)
  ↓
Click does nothing
  ↓
Must select different destination
```

### Opponent Invalid Move Received
```
UDP receives invalid move
  ↓
MessageBox: "Invalid move from opponent"
  ↓
Move rejected, board unchanged
  ↓
Still your turn
```

## Accessibility

### Font Requirements
- Must support Unicode chess symbols
- Recommended: Arial, Segoe UI, Consolas
- Size: 24pt for pieces

### Color Contrast
- White pieces on brown: ✅ Good contrast
- Black pieces on beige: ✅ Good contrast
- Highlights visible on both: ✅ Verified

## Testing Visual Checklist

When testing, verify:
- [ ] Board squares alternate Beige/Brown (NOT White/Crimson)
- [ ] All pieces display as Unicode symbols
- [ ] White pieces: ♔♕♖♗♘♙
- [ ] Black pieces: ♚♛♜♝♞♟
- [ ] Selected piece highlighted Gold
- [ ] Valid empty moves highlighted LightGreen
- [ ] Capturable enemies highlighted IndianRed
- [ ] Turn indicator changes color (Green/Blue/Red)
- [ ] Buttons colored correctly (Orange/Red)
- [ ] Board size: 8×8 grid, 60px squares
- [ ] Window title shows player name

---

This visual guide should help verify the UI implementation matches the requirements!
