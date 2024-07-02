using UnityEngine;

public class JSExitApplication : MonoBehaviour
{
    private bool isFullscreen = false;
    private int screenWidth;
    private int screenHeight;

    void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            ToggleFullscreen();
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                WindowResize();
            #endif
        }
    }

    private void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;

        if (isFullscreen)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            Screen.SetResolution(screenWidth, screenHeight, false);
        }
    }

    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern System.IntPtr GetActiveWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetWindowPos(System.IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

    private const int SW_RESTORE = 9;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int SWP_NOSIZE = 0x0001;

    private void WindowResize()
    {
        System.IntPtr handle = GetActiveWindow();
        ShowWindow(handle, SW_RESTORE);
        SetWindowPos(handle, 0, 0, 0, 800, 600, SWP_SHOWWINDOW | SWP_NOSIZE);
    }
    #endif
}
