using UnityEngine;
using UnityEngine.SceneManagement;


namespace KyleConibear
{
    /// <summary>
    /// The GameManager is the entry point for the application.
    /// It is the first script run and does not require a Unity scene.
    /// It is also the final exit point of the application.
    /// It is responsible for loading the initial scene.
    /// Additionally as the only static class it acts a hub by
    /// connecting objects to the level manager.
    /// 
    /// TODO:
    /// Future responsibility will also include suspending the application.
    /// </summary>
    using static Logger;
    public static class GameManager
    {
        private const string levelName = "GameScene";

        /// <summary>
        /// Once the level has loaded the LevelManager will invoke an Action that the level loading is complete
        /// The levelManager & playerManager member variables will be populated by the objects passed in for mentioned Action
        /// </summary>
        private static LevelManager _level = null;
        public static LevelManager Level => _level;



        /// <summary>
        /// Automatically runs when the application starts and loads the game level
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void LoadGameLevel()
        {
            LevelManager.On_LevelLoaded += LevelLoaded;
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(levelName);
        }

        /// <summary>
        /// Invoked by the LevelManager in the scene once the scene has finished loading
        /// </summary>
        /// <param name="levelManager">The LevelManager in the active scene</param>
        /// <param name="playerManager">The PlayerManager in the active scene</param>
        private static void LevelLoaded(string levelName, LevelManager levelManager)
        {
            if (levelName != string.Empty || levelManager != null)
            {
                Logger.Log(Type.Message, $"Level {levelName} successfully loaded.\n LevelManager={levelManager}.");
                _level = levelManager;
            }
            else
            {
                Logger.Log(Type.Error, $"Level {levelName} loading failed.\n LevelManager={levelManager}.");
                ExitApplication();
            }
        }

        public static void ExitApplication()
        {
            Logger.Log(Type.Message, "Application safely exited.");
            Application.Quit();
        }
    }
}