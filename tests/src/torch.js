// Port to JavaScript of the torch logic (Torch in src/After/Lighting.cs).
// The torch loses fuel over time and turns off when it reaches zero.

class Torch {
  constructor(fuel, onTurnOff) {
    this.fuel = fuel;
    this.lit = true;
    this.onTurnOff = onTurnOff;
  }

  consume(time) {
    if (!this.lit) return false;
    this.fuel = this.fuel - time;
    if (this.fuel <= 0) {
      this.fuel = 0;
      this.lit = false;
      if (this.onTurnOff) this.onTurnOff();
    }
    return this.lit;
  }
}

module.exports = Torch;
