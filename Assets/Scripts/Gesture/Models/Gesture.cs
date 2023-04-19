using System;
using System.Collections.Generic;

[Serializable]
public struct Gesture: IEquatable<Gesture>
{
    public List<Orientation> palmOrientations;
    public List<Orientation> motionOrientations;
    public HandFlags handFlags;

    public Gesture(List<Orientation> palm, List<Orientation> motion, HandFlags handFlags)
    {
        this.palmOrientations = palm;
        this.motionOrientations = motion;
        this.handFlags = handFlags;
    }

    public bool Equals(Gesture other) => palmOrientations == other.palmOrientations
                                         && motionOrientations == other.motionOrientations
                                         && handFlags == other.handFlags;
    public override bool Equals(object obj) => obj is Gesture other && Equals(other);
    public override int GetHashCode() => palmOrientations.GetHashCode() ^ motionOrientations.GetHashCode();

    public static bool operator ==(Gesture left, Gesture right) => left.Equals(right);
    public static bool operator !=(Gesture left, Gesture right) => !(left == right);
}
