using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using androLib.Common.Utility;

namespace WeaponEnchantments.Common.Utility {
    public class Time {
        #region Statics
        public enum Magnitude {
            ticks,
            seconds,
            minutes,
            hours,
        }
        private static IDictionary<Magnitude, (string, string)> MagnitudeStrings = new Dictionary<Magnitude, (string, string)>() {
            { Magnitude.ticks, ("tick", "ticks") },
            { Magnitude.seconds, ("second", "seconds") },
            { Magnitude.minutes, ("minute", "minutes") },
            { Magnitude.hours, ("hour", "hours") },
        }.ToImmutableDictionary();
        
        private static IDictionary<Magnitude, int> Conversions = new Dictionary<Magnitude, int>() {
            { Magnitude.ticks, 60 },
            { Magnitude.seconds, 60 },
            { Magnitude.minutes, 60 },
        }.ToImmutableDictionary();
        private static string MaxIntString = "ever";
        #endregion

        #region Properties
	
	    private float _value = 0;
        public float Value { 
		set => _value = value;
		    get {
			    if (_waitingForEnterWorld)
				    SetUpAutomaticStrengthFromWorldDificulty();
			
			    return _value;
		    }
	    }
	    
	    private int _ticks = 0;
        public int Ticks {
		    set {
                float newValue = value;
                for(int i = 0; i < (int)Mag; i++) {
                    newValue /= Conversions[(Magnitude)i];
				}

                Value = newValue;
                _ticks = value;
            }
		    get {
			    if (_waitingForEnterWorld)
				    SetUpAutomaticStrengthFromWorldDificulty();

                    return _ticks;
		    }
	    }
        public Magnitude Mag;
	    private bool _waitingForEnterWorld = false;
	    private DifficultyStrength _difficultyStrength = null;

        #endregion

        #region Constructors
		
        public Time(uint value, Magnitude mag = Magnitude.ticks) {
            Value = value;
            Mag = mag;
            ReduceSelf();
            Ticks = CalculateTicks();
        }
	    
	    public Time(DifficultyStrength difficultyStrength, Magnitude mag = Magnitude.ticks) {
	        _waitingForEnterWorld = true;
	        _difficultyStrength = difficultyStrength;
                Mag = mag;
        }
        #endregion

        #region Methods
        #region Overwritten Methods
        public override string ToString() {
            if (Ticks >= int.MaxValue) {
                return MaxIntString;
			}

            Tuple<float, Magnitude> maxReducedSelf = MaxReducedSelf();

            return $"{maxReducedSelf.Item1.S()} {(Value >= 2 ? MagnitudeStrings[maxReducedSelf.Item2].Item2 : MagnitudeStrings[maxReducedSelf.Item2].Item1)}";
        }
        #endregion

	    private void SetUpAutomaticStrengthFromWorldDificulty() {
	        int index =  Main.gameMenu ? 0 : _difficultyStrength.AllValues.Length == 4 ? Main.GameMode : 0;
	        _value = _difficultyStrength.AllValues[index];
            ReduceSelf();
            Ticks = CalculateTicks();
	    }
	
        // Simplifies the time as much as possible while lossless. 
        private void ReduceSelf() {
            if (_value < 0) {
                return;
            }
            while (Conversions.ContainsKey(Mag) && _value >= Conversions[Mag] && _value % Conversions[Mag] == 0) {
                _value /= Conversions[Mag];
                Mag += 1;
            }
        }

        // Returns a lossy max simplification
        private Tuple<float, Magnitude> MaxReducedSelf() {
            if (_value < 0) {
                return new Tuple<float, Magnitude>(_value, Mag);
            }
            
            float newValue = _value;
            Magnitude newMag = Mag;
            while (Conversions.ContainsKey(newMag) && newValue > Conversions[newMag] && newValue % Conversions[newMag] == 0) {
                newValue /= Conversions[newMag];
                newMag += 1;
            }
            return new Tuple<float, Magnitude>(newValue, newMag);
        }
      
        // Returns the amount of frames this value represents
        private int CalculateTicks() {
            if (_value < 0) {
                return 0;
            }
            double newValue = _value;
            Magnitude newMag = Mag;
            while (newMag != 0 && Conversions.ContainsKey(newMag - 1)) {
                newMag -= 1;
                newValue *= Conversions[newMag];
            }

            return (int)newValue;
        }
        #endregion

        #region Operator Functions
        public static implicit operator int(Time t) { return t.Ticks; }
        
        public static Time operator *(Time t, int value) {
            return new Time((uint)(t.Value * value), t.Mag);
        }

        public static Time operator *(Time t, float value) {
            uint newFrames = (uint)((float)t * value);
            return new Time(newFrames, Magnitude.ticks);
        }

        public Time Clone() => new Time((uint)Ticks);

        #endregion
    }
}
