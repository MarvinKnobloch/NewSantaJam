using UnityEngine;

namespace Santa
{
    // Irgendwas, das sich bewegt und den Spieler mitnehmen soll.
    public interface IPlatform
    {
        public Vector3 velocity {get;}

        public void OnStepOn();
    }
}
