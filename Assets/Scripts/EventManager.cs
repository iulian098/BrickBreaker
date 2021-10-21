using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public static GameOverEvent gameOverEvent;
    public static AddScoreEvent addScoreEvent;
    public static PowerPickupEvent powerPickupEvent;
    public static StickToPaddleEvent stickToPaddleEvent;
    public static CoinPickUpEvent coinPickUpEvent;
    public static ExplodeEvent explodeEvent;
    public static BallHitEvent ballHitEvent;
    public static BrickDamageEvent brickDamageEvent;

    private void Awake()
    {
        gameOverEvent = new GameOverEvent();
        addScoreEvent = new AddScoreEvent();
        powerPickupEvent = new PowerPickupEvent();
        stickToPaddleEvent = new StickToPaddleEvent();
        coinPickUpEvent = new CoinPickUpEvent();
        explodeEvent = new ExplodeEvent();
        ballHitEvent = new BallHitEvent();
        brickDamageEvent = new BrickDamageEvent();
    }

}
