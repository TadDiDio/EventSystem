using UnityEngine;

public class GameEvents : MonoBehaviour
{
    #region Setup
    public static GameEvents Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    #endregion

    #region Containers
    public InputContainer Input = new InputContainer();
    public PlayerContainer Player = new PlayerContainer();
    public EnemyContainer Enemy = new EnemyContainer();
    public TimelineContainer Timeline = new TimelineContainer();

    #endregion


    public class InputContainer
    {
        public GameEvent Jump = new GameEvent();
        public GameEvent Crouch = new GameEvent();
        public GameEvent<MoveData> Move = new GameEvent<MoveData>();
    }
    public class PlayerContainer
    {
        public GameEvent<float> TookDamage = new GameEvent<float>();
        public GameEvent Died = new GameEvent();
        public GameEvent Respawned = new GameEvent();
    }
    public class EnemyContainer
    {
        public GoombaContainer Goomba = new GoombaContainer();
        public ShitheadContainer Shithead = new ShitheadContainer();

        public class GoombaContainer
        {
            public GameEvent Spawned = new GameEvent();
            public GameEvent Died = new GameEvent();
        }
        public class ShitheadContainer
        {
            public GameEvent Pooped = new GameEvent();
            public GameEvent<int> AteShit = new GameEvent<int>();
        }
    }
    public class TimelineContainer
    {
        public TitleSequenceContainer Title = new TitleSequenceContainer();
        public CutSceneContainer CutScene = new CutSceneContainer();
        public class TitleSequenceContainer
        {

        }
        public class CutSceneContainer
        {
            public BossDeathContainer BossDeath = new BossDeathContainer();
            public RomanceContainer Romance = new RomanceContainer();
            public class BossDeathContainer
            {
                
            }
            public class RomanceContainer
            {
                public GameEvent Kiss = new GameEvent();
                public GameEvent<CuddleData> Cuddle = new GameEvent<CuddleData>();
            }
        }
    }
}