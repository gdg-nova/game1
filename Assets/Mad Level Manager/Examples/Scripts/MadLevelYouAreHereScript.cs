/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using MadLevelManager;

public class MadLevelYouAreHereScript : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    public Vector3 offset = new Vector3(0.37f, 0, 0);
    public float animationAmplitude = 0.02f;
    public float animationSpeed = 3f;
    
    private Transform lastUnlockedTransform;

    private bool initialized;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Update() {
        if (initialized) {
            float xChange = Mathf.Sin(Time.time * animationSpeed) * animationAmplitude;
            transform.position = lastUnlockedTransform.position + offset + new Vector3(xChange, 0, 0);
        }
    }

    void LateUpdate() {
        if (!initialized) {
            Initialize();
        }
    }

    private void Initialize() {
        var currentLayout = MadLevelLayout.current;
        var configuration = MadLevel.activeConfiguration;
        var group = configuration.FindGroupById(currentLayout.configurationGroup);

        var lastUnlockedLevelName = MadLevel.FindLastUnlockedLevelName(group.name);

        var icon = currentLayout.GetIcon(lastUnlockedLevelName);
        lastUnlockedTransform = icon.transform;

        initialized = true;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}