public enum GameState {
    ENDED,      // Game is over
    INIT,       // Game is initializing, no gameplay happening
    NEXT_WAVE,  // Game is in between waves, no action taken but timescale is normal
    PAUSED,     // Gameplay is paused -- timescale is 0
    RUNNING     // Gameplay is happening
}
