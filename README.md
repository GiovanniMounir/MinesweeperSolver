## License
This work is available under the MIT license.

## Developed by
  1. Giovanni Mounir
  2. Paul Ragheb
  3. Beshoy Abd Elsayed
  4. Ahmed Atef
  5. Ahmed Anter

## Dependencies

1. [Emgu CV library](http://www.emgu.com/wiki/index.php/Main_Page) version 3.4.1.

## Features
1. Overlays on Minesweeper cells to indicate presence of bombs or safe cells:
- Blue overlay for safe cells
- Red overlay for bomb cells
2. Contains an option to automatically flag and open cells.
3. Contains an option to automatically restart the game when there is no 100% solution or when the user clicks on a bomb.

## How to use
  - Capture [Button]: to start the program by scanning screen
  - Auto Mouse [CheckBox] to enable autoplay feature
  - Auto Restart [CheckBox] to enable restarting the game immediately
  - Refresh Frequency [ms] to adjust scannning frequency
  - F1 [Hot Key] to disable Auto-Mouse
  - X [Button] to close the program
  
## Download

To download the executables, please click [here](https://github.com/GiovanniMounir/MinesweeperSolver/releases/download/1.0/MinesweeperSolver_x86_64.zip) to download the latest release and run "MinesweeperSolver.exe" directly.

## Demos

You can view recordings for this software in the "Demos" folder.

## Limitations
1. The program will not work correctly when the DPI/scaling factor is not 1 (i.e 100%). Make sure that scale is set to 100% in the display settings on Microsoft Windows. An error message is shown otherwise.
2. Cells marked unknown "?" are not properly interpreted by the algorithm. Avoid marking cells as unknown "?" for best accuracy.
3. For best performance, the software only detects the window position of the game after clicking "Capture". Moving the game window after clicking "Capture" will capture the old position: clicking "Capture" again is necessary after moving the game window for best results.
4. Click on "Capture" after changing the settings to ensure that your settings were saved.
5. Only works with the classic Minesweeper design, such as [WinmineXP](http://www.minesweeper.info/downloads/WinmineXP.html) or [http://minesweeperonline.com/](http://minesweeperonline.com/)
