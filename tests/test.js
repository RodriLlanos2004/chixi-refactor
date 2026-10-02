const { nextState } = require('./src/playerStates');
const Torch = require('./src/torch');

describe('playerStates', () => {
  test('Verify player walks when pressing move', () => {
    // Arrange: initializing inputs & parameters
    const current = 'idle';
    const action = 'move';

    // Act
    const result = nextState(current, action);

    // Assert
    expect(result).toBe('walking');
  });

  test('Verify player jumps when pressing jump', () => {
    // Arrange
    const current = 'walking';
    const action = 'jump';

    // Act
    const result = nextState(current, action);

    // Assert
    expect(result).toBe('jumping');
  });

  test('Verify player stays the same with unknown action', () => {
    // Arrange
    const current = 'walking';
    const action = 'dance';

    // Act
    const result = nextState(current, action);

    // Assert
    expect(result).toBe('walking');
  });
});

describe('torch', () => {
  test('Verify torch stays lit while it has fuel', () => {
    // Arrange: instantiating objects on SUT
    const torch = new Torch(10);

    // Act
    const lit = torch.consume(4);

    // Assert
    expect(lit).toBe(true);
    expect(torch.fuel).toBe(6);
  });

  test('Verify torch turns off when fuel runs out', () => {
    // Arrange: configuring dependencies with mocks
    const onTurnOff = jest.fn();
    const torch = new Torch(5, onTurnOff);

    // Act
    const lit = torch.consume(5);

    // Assert
    expect(lit).toBe(false);
    expect(torch.fuel).toBe(0);
    expect(onTurnOff).toHaveBeenCalledTimes(1);
  });
});
