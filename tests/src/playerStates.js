// Port to JavaScript of the player state machine (src/After/PlayerStates.cs).
// Only the transition logic is moved, so it can be tested with Jest.

function nextState(current, action) {
  if (action === 'jump') return 'jumping';
  if (action === 'stop') return 'idle';
  if (current === 'idle' && action === 'move') return 'walking';
  return current;
}

module.exports = { nextState };
