using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility {
    public class Time {
        #region Statics
        public enum Magnitude {
            ticks,
            seconds,
            minutes,
            hours,
        }
        private static IDictionary<Magnitude, string> MagnitudeStrings = new Dictionary<Magnitude, string>() {
            { Magnitude.ticks, "ticks" },
            { Magnitude.seconds, "seconds" },
            { Magnitude.minutes, "minutes" },
            { Magnitude.hours, "hours" },
        }.ToImmutableDictionary();
        
        private static IDictionary<Magnitude, int> Conversions = new Dictionary<Magnitude, int>() {
            { Magnitude.ticks, 60 },
            { Magnitude.seconds, 60 },
            { Magnitude.minutes, 60 },
        }.ToImmutableDictionary();
        private static string IndefiniteString = "indefinite";
        private static string MaxIntString = "ever";
        #endregion

        #region Properties

        public double Value;
        public int Ticks = 0;
        public Magnitude Mag;

        #endregion

        #region Constructors
        public Time(double value, Magnitude mag = Magnitude.ticks) {
            Value = value;
            double temp = value * 10000;
            Mag = mag;
            ReduceSelf();
            Ticks = CalculateTicks();
        }
        #endregion

        #region Methods
        #region Overwritten Methods
        public override string ToString() {
            if (Value < 0) {
                return IndefiniteString;
            }
            else if (Ticks == int.MaxValue) {
                return MaxIntString;
			}

            var maxReducedSelf = MaxReducedSelf();
            return $"{Math.Round(maxReducedSelf.Item1, 1)} {MagnitudeStrings[maxReducedSelf.Item2]}";
        }
        #endregion

        // Simplifies the time as much as possible while lossless. 
        private void ReduceSelf() {
            if (Value < 0) {
                return;
            }
            while (Conversions.ContainsKey(Mag) && Value >= Conversions[Mag] && Value % Conversions[Mag] == 0) {
                Value /= Conversions[Mag];
                Mag += 1;
            }
        }

        // Returns a lossy max simplification
        private Tuple<double, Magnitude> MaxReducedSelf() {
            if (Value < 0) {
                return new Tuple<double, Magnitude>(Value, Mag);
            }
            
            double newValue = Value;
            Magnitude newMag = Mag;
            while (Conversions.ContainsKey(newMag) && newValue > Conversions[newMag] && newValue % Conversions[newMag] == 0) {
                newValue /= Conversions[newMag];
                newMag += 1;
            }
            return new Tuple<double, Magnitude>(newValue, newMag);
        }
      
        // Returns the amount of frames this value represents
        private int CalculateTicks() {
            if (Value < 0) {
                return (int)Value;
            }
            double newValue = Value;
            Magnitude newMag = Mag;
            while (newMag != 0 && Conversions.ContainsKey(newMag - 1)) {
                newMag -= 1;
                newValue *= Conversions[newMag];
            }

            double temp = newValue * 10000;

            return (int)newValue;
        }
        #endregion

        #region Operator Functions
        public static implicit operator int(Time t) { return t.Ticks; }
        
        public static Time operator *(Time t, int value) {
            return new Time(t.Value * value, t.Mag);
        }

        public static Time operator *(Time t, float value) {
            int newFrames = (int)((int)t * value);
            return new Time(newFrames, Magnitude.ticks);
        }

        #endregion
    }
}
