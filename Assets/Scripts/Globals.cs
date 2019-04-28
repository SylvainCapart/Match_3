public static class Globals
{
    // Horzontal size of the screen
    public const int SCREEN_HOR_SIZE = 564;

    // Vertical size of the screen
    public const int SCREEN_VER_SIZE = 900;

    // Delay between tile apparition during level loading
    public const float TILE_INIT_DELAY = 0.01f;

    // Minimum requested amount of Items selected to score
    public const int MIN_CHAIN_NB = 3;

    // Maximum allowed actions
    public const int MAX_MOVES_NB = 10;

    // Probability distribution for the Items
    public const int DISTRIB_ITEM_BLUE = 25;
    public const int DISTRIB_ITEM_YELLOW = 50;
    public const int DISTRIB_ITEM_GREEN = 70;
    public const int DISTRIB_ITEM_ORANGE = 90;
    public const int DISTRIB_ITEM_RED = 100;

    // Probability distribution for the Bonus
    public const int DISTRIB_BONUS_X2 = 60;
    public const int DISTRIB_BONUS_BOMB = 83;
    public const int DISTRIB_BONUS_ERAS_COL = 94;

    // Coefficient used to reward the scoring of longer chains
    public const float COMBO_MULTIPLIER = 0.1f;

    // Points attributed to each items
    public const int POINTS_ITEM_BLUE = 5;
    public const int POINTS_ITEM_YELLOW = 5;
    public const int POINTS_ITEM_GREEN = 10;
    public const int POINTS_ITEM_ORANGE = 10;
    public const int POINTS_ITEM_RED = 15;

}