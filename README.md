# TinyRowGame


## Protocol documentation


| Event | Server sends | Client sends | Description | What's suppose to happen |
|-----|-----|-----|-----|---|---|
| Client connect | -> Init | | Contains user number and initial grid | The client renders the grid |
| Grid change | -> Grid | | The grid has changed (only placed points). Like other players have placed marks or the game state has changed | The client renders the grid |
| Place | | <- place | The user has placed a mark | Update the GUI |
| Local user turn | -> Turn | | It's the receipient users (your) turn | Display a message and allow input. The server expects a Place message within 5 seconds. |
| Other user turn | -> OtherUserTurn | | It's another users turn | Display a message and wait |
| Player wins | -> Winner | | A player has won | Display the Game over message |
