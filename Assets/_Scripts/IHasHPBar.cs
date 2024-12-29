using System;
using UnityEngine;

public interface IHasHPBar {

    public event EventHandler<OnHPChangedEventArgs> OnHPChanged;

    public class OnHPChangedEventArgs : EventArgs {
        public float HPNormalized;
        public float MPNormalized;
        public float levelNormalized;
    }
}